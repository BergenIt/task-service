using DatabaseExtension.Translator;

using Newtonsoft.Json;

using Quartz;

using TaskService;
using TaskService.Core.AuditWriter;
using TaskService.Core.Elasticsearch;
using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models;
using TaskService.Core.Options;
using TaskService.Core.SchedulerWorkers.JobDetailBuilder;
using TaskService.Core.SchedulerWorkers.QuartzMigrator;
using TaskService.Core.SchedulerWorkers.ScheduleGetter;
using TaskService.Core.SchedulerWorkers.ScheduleManager;
using TaskService.Core.TaskExecutor;
using TaskService.Core.TaskManagers.FireAndForgetManager;
using TaskService.Core.TaskManagers.SystemTaskManager;
using TaskService.Core.TaskManagers.UserScheduleManager;
using TaskService.Core.TaskRegistry;
using TaskService.Core.TaskSystemImplementations.Http;
using TaskService.Elasticsearch;
using TaskService.Services;
using TaskService.StartupConfigure;

using UserService.Proto;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
{
    Converters = new List<JsonConverter>() { new JobMergedDataConverter() },
};

builder.BaseConfigure(out ProjectOptions projectOptions, out ITranslator translator);
builder.Services.AddGrpcRouteParser(typeof(AuditService));
builder.Services.AddQuartz(translator, projectOptions, out IDictionary<JobKey, BackTaskRegistry> jobs);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddGrpc(o => o.Interceptors.Add<ApplicationInterceptor>());
builder.Services.AddSingleton<ITranslator>(translator);
builder.Services.AddElasticClient(projectOptions);
builder.Services.AddScoped<IRouteParser<HttpRoute>, HttpRouteParser>();
builder.Services.AddScoped<IElasticsearchMigrator, ElasticsearchMigrator>();
builder.Services.AddScoped<IElasticsearchGetter, ElasticsearchGetter>();
builder.Services.AddScoped<IElasticsearchWorker, ElasticsearchWorker>();
builder.Services.AddScoped<ILifecycleIndexManager, LifecycleIndexManager>();
builder.Services.AddGrpcClient<AuditService.AuditServiceClient>(c => c.Address = projectOptions.UserServiceUri);
builder.Services.AddTransient<IAuditGrpcСlient>(s => s.GetRequiredService<AuditService.AuditServiceClient>());
builder.Services.AddScoped<IJwtParser, JwtParser>();
builder.Services.AddScoped<IAuditWriter, AuditWriter>();
builder.Services.AddScoped<IJobDetailBuilder, JobDetailBuilder>();
builder.Services.AddScoped<IScheduleManager, ScheduleManager>();
builder.Services.AddScoped<IScheduleGetter, ScheduleGetter>();
builder.Services.AddScoped<IFireAndForgetManager, FireAndForgetManager>();
builder.Services.AddScoped<ISystemTaskManager, SystemTaskManager>();
builder.Services.AddScoped<IUserScheduleManager, UserScheduleManager>();
builder.Services.AddTransient<IQuartzMigrator, QuartzMigrator>();
builder.Services.AddHttpContextAccessor();

WebApplication app = builder.Build();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapGrpcService<FireAndForgetServices>();
app.MapGrpcService<SystemTaskServices>();
app.MapGrpcService<UserScheduleServices>();
app.MapGrpcService<TaskRegisterServices>();
app.MapGrpcService<LifecycleIndexServices>();
app.MapGrpcService<TaskDescriptorServices>();

await app.Services.ElasticsearchMigrate();
await app.Services.QuartzMigrate(jobs, projectOptions.WriteLevel);

app.Run();
