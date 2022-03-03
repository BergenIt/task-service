
using DatabaseExtension;
using DatabaseExtension.Translator;

using Newtonsoft.Json;

using TaskService.Core.Models;
using TaskService.Core.TaskExecutor.TaskExecutorImplementations;
using TaskService.Core.SystemImplimentationType;

using YamlDotNet.Serialization;
using TaskService.Core.SystemImplimentationType.Models;
using TaskService.Core.Options;

namespace TaskService.Core.TaskRegistry;

public static class YamlTaskRegistryMethods
{
    private const string YamlType = ".yaml";

    public static IServiceTaskRegistry AddJobFromYaml(this IServiceTaskRegistry taskRegistry, ITranslator translator, ProjectOptions projectOptions)
    {
        JobsDescriptior jobsDescriptior = YamlLoad(projectOptions.UserJobsYaml, projectOptions.ShadowJobsYaml, projectOptions.SystemJobsYaml);

        if (jobsDescriptior.ShadowJobs is not null)
        {
            foreach (KeyValuePair<string, ShadowJobYamlSchema> jobs in jobsDescriptior.ShadowJobs)
            {
                string key = AddTask(taskRegistry, TaskType.ShadowTask, jobs.Key, jobs.Value, jobs.Value.Interval, jobs.Value.JsonData);
                translator.AddTranslate<Quartz.IJob>(key, jobs.Key);
            }
        }

        if (jobsDescriptior.SystemJobs is not null)
        {
            foreach (KeyValuePair<string, SystemJobYamlSchema> jobs in jobsDescriptior.SystemJobs)
            {
                string key = AddTask(taskRegistry, TaskType.SystemTask, jobs.Key, jobs.Value, jobs.Value.Interval, jobs.Value.JsonData);
                translator.AddTranslate<Quartz.IJob>(key, jobs.Key);
            }
        }

        if (jobsDescriptior.UserJobs is not null)
        {
            foreach (KeyValuePair<string, UserJobYamlSchema> jobs in jobsDescriptior.UserJobs)
            {
                string key = AddTask(taskRegistry, TaskType.UserTask, jobs.Key, jobs.Value);
                translator.AddTranslate<Quartz.IJob>(key, jobs.Key);
            }
        }

        return taskRegistry;
    }

    private static string AddTask(IServiceTaskRegistry taskRegistry, TaskType taskType, string name, JobYamlSchema jobYamlSchema, TimeSpan? interval = null, object? jsonData = null)
    {
        JobYamlSchemaToSystemExecutorTypes(jobYamlSchema, out IEnumerable<Type> genericTypes, out IEnumerable<Type> diTypes);

        Dictionary<string, string> jobRootData = new()
        {
            { nameof(JobMergedData.SenderRoute), JsonConvert.SerializeObject(jobYamlSchema.Sender.Router) },
        };

        if (jobYamlSchema.Validator is not null)
        {
            jobRootData.Add(nameof(JobMergedData.ValidatorRoute), JsonConvert.SerializeObject(jobYamlSchema.Validator.Router));
        }

        if (jobYamlSchema.Selector is not null)
        {
            jobRootData.Add(nameof(JobMergedData.SelectorRoute), JsonConvert.SerializeObject(jobYamlSchema.Selector.Router));
        }

        if (jsonData is not null)
        {
            Dictionary<string, string> triggerProperties = new()
            {
                { nameof(JobMergedData.Data), JsonConvert.SerializeObject(jsonData) },
            };

            jsonData = triggerProperties;
        }

        string jobName = GuidCreator.CreateFrom(name).ToString();

        _ = taskRegistry.AddTask(
            taskType,
            jobName,
            jobYamlSchema.Schema,
            interval,
            jsonData,
            jobRootData,
            JobYamlSchemaToTaskExecutor(jobYamlSchema),
            genericTypes,
            diTypes.Select(t => new TypeDesctriptor(t, null)).ToArray()
        );

        return jobName;
    }

    private static void JobYamlSchemaToSystemExecutorTypes(JobYamlSchema jobYamlSchema, out IEnumerable<Type> genericTypes, out IEnumerable<Type> diTypes)
    {
        List<Type> dataTypes = new();
        List<Type> types = new();

        if (jobYamlSchema.Validator is not null)
        {
            types.Add(jobYamlSchema.Validator.Type.GetValidatorType());
        }
        if (jobYamlSchema.Selector is not null)
        {
            types.Add(jobYamlSchema.Selector.Type.GetSelectorType());
            dataTypes.Add(jobYamlSchema.Selector.Type.GetDataType());
        }

        types.Add(jobYamlSchema.Sender.Type.GetSenderType());
        dataTypes.Add(jobYamlSchema.Sender.Type.GetDataType());
        diTypes = types.ToArray();
        types.AddRange(dataTypes);
        genericTypes = types;
    }

    private static Type JobYamlSchemaToTaskExecutor(JobYamlSchema jobYamlSchema)
    {
        switch (jobYamlSchema.Selector)
        {
            case null when jobYamlSchema.Validator is null: return typeof(TaskExecutor<,>);
            case not null when jobYamlSchema.Validator is null: return typeof(TaskExecutor<,,,>);
            case null: return typeof(TaskExecutor<,,>);
            case not null: return typeof(TaskExecutor<,,,,>);
        }
    }

    private static JobsDescriptior YamlLoad(params string[] configPath)
    {
        string json = string.Empty;

        foreach (string path in configPath)
        {
            using StreamReader reader = new(path);

            string yaml = reader.ReadToEnd() + '\n';

            reader.Close();

            if (path.Contains(YamlType))
            {
                using StringReader stringReader = new(yaml);

                IDeserializer deserializer = new DeserializerBuilder().Build();
                object? yamlObject = deserializer.Deserialize(stringReader);

                if (yamlObject is null)
                {
                    throw new ArgumentNullException(nameof(configPath));
                }

                ISerializer serializer = new SerializerBuilder()
                    .JsonCompatible()
                    .Build();

                yaml = serializer.Serialize(yamlObject);
            }

            json += yaml.Trim()[1..][..^1] + ',';
        }

        json = '{' + json[..^1] + '}';

        return JsonConvert.DeserializeObject<JobsDescriptior>(json)
            ?? throw new InvalidCastException($"Not parse yaml from: {string.Join(" ", configPath)}");
    }
}
