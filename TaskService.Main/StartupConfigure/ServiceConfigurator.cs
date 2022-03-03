
using DatabaseExtension.Translator;

using Elasticsearch.Net;

using Grpc.Core;

using Microsoft.Net.Http.Headers;

using Nest;

using Quartz;

using Serilog;

using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models;
using TaskService.Core.Options;
using TaskService.Core.SchedulerWorkers.QuartzMigrator;
using TaskService.Core.TaskExecutor;
using TaskService.Core.TaskRegistry;
using TaskService.Core.TaskSystemImplementations.Http;
using TaskService.Npsql;

namespace TaskService.StartupConfigure;

/// <summary>
/// Конфигурация serviceCollection
/// </summary>
public static class ServiceConfigurator
{
    /// <summary>
    /// Инит опций и транслятора из конфигурации
    /// Добавление логирования
    /// Миграции
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="projectOptions"></param>
    /// <param name="translator"></param>
    public static void BaseConfigure(this WebApplicationBuilder builder, out ProjectOptions projectOptions, out ITranslator translator)
    {
        projectOptions = new(builder.Configuration);

        builder.Host.UseSerilog((_, c) => c.ReadFrom.Configuration(builder.Configuration));

        NpsqlCreator npsqlCreator = new(projectOptions);
        npsqlCreator.CreateDatabase();

        translator = new Translator(projectOptions.TranslatorFiles);
    }

    /// <summary>
    /// Добавления эластик клиента в di
    /// </summary>
    /// <param name="serviceDescriptors"></param>
    /// <param name="projectOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddElasticClient(this IServiceCollection serviceDescriptors, ProjectOptions projectOptions) =>
        serviceDescriptors.AddScoped<IElasticClient>(p =>
        {
            ConnectionSettings connectionSettings = new(new SingleNodeConnectionPool(projectOptions.ElasticsearchUri));

            _ = connectionSettings.DisableDirectStreaming();

            if (!string.IsNullOrWhiteSpace(projectOptions.ElasticsearchToken))
            {
                _ = connectionSettings.GlobalHeaders(new() { { HeaderNames.Authorization, projectOptions.ElasticsearchToken } });
            }

            return new Nest.ElasticClient(connectionSettings);
        });

    /// <summary>
    /// Добавление всех грпц клиентов, которые найдем в ассамблее
    /// </summary>
    /// <param name="serviceDescriptors"></param>
    /// <param name="assembleMarkers"></param>
    /// <returns></returns>
    public static IServiceCollection AddGrpcRouteParser(this IServiceCollection serviceDescriptors, params Type[] assembleMarkers)
    {
        IEnumerable<Type> grpcClientTypes = assembleMarkers
            .Select(t => t.Assembly)
            .SelectMany(a =>
                a.GetTypes()
                .Where(t => t.BaseType?.BaseType == typeof(ClientBase))
            );

        GrpcRouteParser grpcRouteParser = new(grpcClientTypes);

        return serviceDescriptors.AddSingleton<IRouteParser<GrpcRoute>>(grpcRouteParser);
    }

    /// <summary>
    /// Добавление кварца и фоновых задач в него
    /// </summary>
    /// <param name="serviceDescriptors"></param>
    /// <param name="translator"></param>
    /// <param name="projectOptions"></param>
    /// <param name="jobs"></param>
    /// <returns></returns>
    public static IServiceCollection AddQuartz(this IServiceCollection serviceDescriptors, ITranslator translator, ProjectOptions projectOptions, out IDictionary<JobKey, BackTaskRegistry> jobs)
    {
        TaskRegistryRecord taskRegistryRecord = new(
            serviceDescriptors,
            new Dictionary<string, TaskRegistryDescriptor>(),
            new Dictionary<string, BackTaskRegistry>(),
            new Dictionary<string, BackTaskRegistry>()
        );

        return taskRegistryRecord.AddQuartz(translator, projectOptions, out jobs);
    }

    /// <summary>
    /// Добавление кварца и фоновых задач в него
    /// </summary>
    /// <param name="taskRegistry"></param>
    /// <param name="translator"></param>
    /// <param name="projectOptions"></param>
    /// <param name="jobs"></param>
    /// <returns></returns>
    public static IServiceCollection AddQuartz(this IServiceTaskRegistry taskRegistry, ITranslator translator, ProjectOptions projectOptions, out IDictionary<JobKey, BackTaskRegistry> jobs)
    {
        taskRegistry.AddJobFromYaml(translator, projectOptions);

        taskRegistry.ServiceDescriptors.AddQuartzHostedService(o => o.WaitForJobsToComplete = true);
        taskRegistry.ServiceDescriptors.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.UsePersistentStore(c =>
            {
                c.UseProperties = true;
                c.UsePostgres(p => p.ConnectionString = projectOptions.PsqlDbConnectionString);
                c.UseJsonSerializer();
            });
        });

        jobs = new Dictionary<JobKey, BackTaskRegistry>();

        Dictionary<string, Type> keyValuePairs = new();

        foreach (KeyValuePair<string, TaskRegistryDescriptor> item in taskRegistry.UserJobs)
        {
            BackTaskRegistry backTaskRegistry = new(item.Value.TaskType, item.Value.Schema, TimeSpan.Zero, new Dictionary<string, string>(), item.Value.RootData);
            jobs.Add(new(item.Key, TaskType.UserTask.ToString()), backTaskRegistry);
            keyValuePairs.Add(item.Key, item.Value.TaskType);
        }

        foreach (KeyValuePair<string, BackTaskRegistry> item in taskRegistry.ShadowJobs)
        {
            jobs.Add(new(item.Key, TaskType.ShadowTask.ToString()), item.Value);
            keyValuePairs.Add(item.Key, item.Value.TaskType);
        }

        foreach (KeyValuePair<string, BackTaskRegistry> item in taskRegistry.SystemJobs)
        {
            jobs.Add(new(item.Key, TaskType.SystemTask.ToString()), item.Value);
            keyValuePairs.Add(item.Key, item.Value.TaskType);
        }

        taskRegistry.ServiceDescriptors.AddSingleton<IJobKeys>(new JobKeys(keyValuePairs));
        taskRegistry.ServiceDescriptors.AddSingleton<ITaskRegistry>(taskRegistry);

        return taskRegistry.ServiceDescriptors;
    }

    /// <summary>
    /// Миграции по ключам данных в эластик
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static async Task ElasticsearchMigrate(this IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        IElasticsearchMigrator elasticsearchMigrator = scope.ServiceProvider.GetRequiredService<IElasticsearchMigrator>();

        IJobKeys jobKeys = scope.ServiceProvider.GetRequiredService<IJobKeys>();

        await elasticsearchMigrator.MigrateDataStreamAsync<ErrorExecutorLog>(
            nameof(ExecutorLog),
            jobKeys.TypeHumanNames.Select(t => t.Key)
        );
    }

    /// <summary>
    /// Миграции задач в квартзе
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="jobs"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static async Task QuartzMigrate(this IServiceProvider serviceProvider, IDictionary<JobKey, BackTaskRegistry> jobs, WriteLevel level)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        IQuartzMigrator quartzMigrator = scope.ServiceProvider.GetRequiredService<IQuartzMigrator>();

        await quartzMigrator.Migrate(jobs, level);
    }
}
