
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TaskService.Core.Models;
using TaskService.Core.SchedulerWorkers.ScheduleGetter;
using TaskService.Core.TaskExecutor;
using TaskService.Core.TaskExecutor.TaskExecutorImplementations;

namespace TaskService.Core.TaskRegistry;

public static class InternalTaskRegistryMethods
{
    internal static IServiceTaskRegistry AddTask<TValidator, TSelector, TSender, TData, TMsg>(this
            IServiceCollection serviceDescriptors,
            TaskType taskType,
            string? name,
            TimeSpan? timeSpan,
            TData? data,
            Func<IServiceProvider, TValidator>? validatorImplementationFactory,
            Func<IServiceProvider, TSelector>? selectorImplementationFactory,
            Func<IServiceProvider, TSender>? senderImplementationFactory
        )
        where TValidator : class, ITaskValidator<TData>
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TMsg : class
        where TData : class
    {
        TaskRegistryRecord taskRegistry = new(serviceDescriptors, new Dictionary<string, TaskRegistryDescriptor>(), new Dictionary<string, BackTaskRegistry>(), new Dictionary<string, BackTaskRegistry>());

        return taskRegistry.AddTask<TValidator, TSelector, TSender, TData, TMsg>(taskType, name, timeSpan, data, validatorImplementationFactory, selectorImplementationFactory, senderImplementationFactory);
    }

    internal static IServiceTaskRegistry AddTask<TSelector, TSender, TData, TMsg>(this
            IServiceCollection serviceDescriptors,
            TaskType taskType,
            string? name,
            TimeSpan? timeSpan,
            TData? data,
            Func<IServiceProvider, TSelector>? implementationFactory,
            Func<IServiceProvider, TSender>? senderImplementationFactory
        )
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        TaskRegistryRecord taskRegistry = new(serviceDescriptors, new Dictionary<string, TaskRegistryDescriptor>(), new Dictionary<string, BackTaskRegistry>(), new Dictionary<string, BackTaskRegistry>());

        return taskRegistry.AddTask<TSelector, TSender, TData, TMsg>(taskType, name, timeSpan, data, implementationFactory, senderImplementationFactory);
    }

    internal static IServiceTaskRegistry AddTask<TValidator, TSender, TData>(this
            IServiceCollection serviceDescriptors,
            TaskType taskType,
            string? name,
            TimeSpan? timeSpan,
            TData? data,
            Func<IServiceProvider, TValidator>? implementationFactory,
            Func<IServiceProvider, TSender>? senderImplementationFactory
        )
        where TValidator : class, ITaskValidator<TData>
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        TaskRegistryRecord taskRegistry = new(serviceDescriptors, new Dictionary<string, TaskRegistryDescriptor>(), new Dictionary<string, BackTaskRegistry>(), new Dictionary<string, BackTaskRegistry>());

        return taskRegistry.AddTask<TValidator, TSender, TData>(taskType, name, timeSpan, data, implementationFactory, senderImplementationFactory);
    }

    internal static IServiceTaskRegistry AddTask<TSender, TData>(this
            IServiceCollection serviceDescriptors,
            TaskType taskType,
            string? name,
            TimeSpan? timeSpan,
            TData? data,
            Func<IServiceProvider, TSender>? implementationFactory
        )
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        TaskRegistryRecord taskRegistry = new(serviceDescriptors, new Dictionary<string, TaskRegistryDescriptor>(), new Dictionary<string, BackTaskRegistry>(), new Dictionary<string, BackTaskRegistry>());

        return taskRegistry.AddTask<TSender, TData>(taskType, name, timeSpan, data, implementationFactory);
    }

    internal static IServiceTaskRegistry AddTask<TSender, TData>(this
            IServiceTaskRegistry taskRegistry,
            TaskType taskType,
            string? name,
            TimeSpan? timeSpan,
            TData? data,
            Func<IServiceProvider, TSender>? implementationFactory
        )
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        Type[] executorGenericTypes = new Type[]
        {
            typeof(TSender),
            typeof(TData),
        };

        return AddTask(
            taskRegistry,
            taskType,
            name,
            null,
            timeSpan,
            data,
            null,
            typeof(TaskExecutor<,>),
            executorGenericTypes,
            new TypeDesctriptor(typeof(TSender), implementationFactory)
        );
    }

    internal static IServiceTaskRegistry AddTask<TSelector, TSender, TData, TMsg>(this
            IServiceTaskRegistry taskRegistry,
            TaskType taskType,
            string? name,
            TimeSpan? timeSpan,
            TData? data,
            Func<IServiceProvider, TSelector>? implementationFactory,
            Func<IServiceProvider, TSender>? senderImplementationFactory
        )
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        List<Type> executorGenericTypes = new()
        {
            typeof(TSelector),
            typeof(TSender),
            typeof(TData),
            typeof(TMsg),
        };

        Type executorType = typeof(TaskExecutor<,,,>);

        return AddTask(
            taskRegistry,
            taskType,
            name,
            null,
            timeSpan,
            data,
            null,
            executorType,
            executorGenericTypes,
            new(typeof(TSelector), implementationFactory),
            new(typeof(TSender), senderImplementationFactory)
        );
    }

    internal static IServiceTaskRegistry AddTask<TValidator, TSender, TData>(this
            IServiceTaskRegistry taskRegistry,
            TaskType taskType,
            string? name,
            TimeSpan? timeSpan,
            TData? data,
            Func<IServiceProvider, TValidator>? implementationFactory,
            Func<IServiceProvider, TSender>? senderImplementationFactory
        )
        where TValidator : class, ITaskValidator<TData>
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        List<Type> executorGenericTypes = new()
        {
            typeof(TValidator),
            typeof(TSender),
            typeof(TData),
        };

        Type executorType = typeof(TaskExecutor<,,>);

        return AddTask(
            taskRegistry,
            taskType,
            name,
            null,
            timeSpan,
            data,
            null,
            executorType,
            executorGenericTypes,
            new(typeof(TValidator), implementationFactory),
            new(typeof(TSender), senderImplementationFactory)
        );
    }

    internal static IServiceTaskRegistry AddTask<TValidator, TSelector, TSender, TData, TMsg>(this
            IServiceTaskRegistry taskRegistry,
            TaskType taskType,
            string? name,
            TimeSpan? timeSpan,
            TData? data,
            Func<IServiceProvider, TValidator>? validatorImplementationFactory,
            Func<IServiceProvider, TSelector>? selectorImplementationFactory,
            Func<IServiceProvider, TSender>? senderImplementationFactory
        )
        where TValidator : class, ITaskValidator<TData>
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        Type[] executorGenericTypes = new Type[]
        {
            typeof(TValidator),
            typeof(TSelector),
            typeof(TSender),
            typeof(TData),
            typeof(TMsg),
        };

        return AddTask(
            taskRegistry,
            taskType,
            name,
            null,
            timeSpan,
            data,
            null,
            typeof(TaskExecutor<,,,,>),
            executorGenericTypes,
            new(typeof(TValidator), validatorImplementationFactory),
            new(typeof(TSelector), selectorImplementationFactory),
            new(typeof(TSender), senderImplementationFactory)
        );
    }

    public static IServiceTaskRegistry AddTask(this
            IServiceTaskRegistry taskRegistry,
            TaskType taskType,
            string? name,
            object? dataSchema,
            TimeSpan? timeSpan,
            object? data,
            IDictionary<string, string>? jobRootData,
            Type executorType,
            IEnumerable<Type> genericTypes,
            params TypeDesctriptor[] diTypes
        )
    {
        name ??= executorType.GetExecutorHumanName();
        jobRootData ??= new Dictionary<string, string>();

        IServiceCollection serviceCollection = taskRegistry.ServiceDescriptors;

        foreach (TypeDesctriptor diType in diTypes)
        {
            if (!serviceCollection.Any(d => d.ServiceType == diType.Type && d.Lifetime == ServiceLifetime.Scoped))
            {
                _ = diType.ImplementationFactory is null
                    ? serviceCollection.AddScoped(diType.Type)
                    : serviceCollection.AddScoped(diType.Type, diType.ImplementationFactory);
            }
        }

        Type taskExecutorType = executorType
            .MakeGenericType(genericTypes.ToArray());

        _ = serviceCollection.AddScoped(taskExecutorType);

        string jsonSchema = JsonConvert.SerializeObject(dataSchema ?? new { });

        if (taskType is not TaskType.UserTask)
        {
            if (!timeSpan.HasValue)
            {
                throw new ArgumentNullException(nameof(timeSpan));
            }
            IDictionary<string, string> dataMap = ObjectToMap(data);

            if (taskType is TaskType.ShadowTask)
            {
                taskRegistry.ShadowJobs.Add(name, new(taskExecutorType, jsonSchema, timeSpan.Value, dataMap, jobRootData));
            }
            else
            {
                taskRegistry.SystemJobs.Add(name, new(taskExecutorType, jsonSchema, timeSpan.Value, dataMap, jobRootData));
            }
        }
        else
        {
            taskRegistry.UserJobs.Add(name, new(taskExecutorType, jsonSchema, jobRootData));
        }

        return taskRegistry;
    }

    private static IDictionary<string, string> ObjectToMap(object? data)
    {
        object notNullData = data ?? new { };

        string json = JsonConvert.SerializeObject(notNullData);

        IDictionary<string, JToken?> jsonObject = JObject.Parse(json);

        IDictionary<string, string> dataMap = jsonObject.ToDictionary(
            k => k.Key,
            v => v.Value?.Value<string>() ?? string.Empty
        );
        return dataMap;
    }
}
