using Microsoft.Extensions.DependencyInjection;

using TaskService.Core.Models;
using TaskService.Core.TaskExecutor;

namespace TaskService.Core.TaskRegistry;

public static class TaskRegistryMethods
{
    public static IServiceTaskRegistry AddUserTask<TValidator, TSelector, TSender, TData, TMsg>(this
        IServiceCollection serviceDescriptors,
        string? name = null,
        Func<IServiceProvider, TValidator>? validatorImplementationFactory = null,
        Func<IServiceProvider, TSelector>? selectorImplementationFactory = null,
        Func<IServiceProvider, TSender>? senderImplementationFactory = null
    )
        where TValidator : class, ITaskValidator<TData>
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TValidator, TSelector, TSender, TData, TMsg>(TaskType.UserTask, name, null, null, validatorImplementationFactory, selectorImplementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddUserTask<TValidator, TSender, TData>(this
            IServiceCollection serviceDescriptors,
            string? name = null,
            Func<IServiceProvider, TValidator>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TValidator, TSender, TData>(TaskType.UserTask, name, null, null, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddUserTask<TSelector, TSender, TData, TMsg>(this
            IServiceCollection serviceDescriptors,
            string? name = null,
            Func<IServiceProvider, TSelector>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TSelector, TSender, TData, TMsg>(TaskType.UserTask, name, null, null, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddUserTask<TSender, TData>(this
            IServiceCollection serviceDescriptors,
            string? name = null,
            Func<IServiceProvider, TSender>? implementationFactory = null
        )
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TSender, TData>(TaskType.UserTask, name, null, null, implementationFactory);
    }

    public static IServiceTaskRegistry AddShadowTask<TValidator, TSelector, TSender, TData, TMsg>(this
            IServiceCollection serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TValidator>? validatorImplementationFactory = null,
            Func<IServiceProvider, TSelector>? selectorImplementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TValidator, TSelector, TSender, TData, TMsg>(TaskType.ShadowTask, name, interval, data, validatorImplementationFactory, selectorImplementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddShadowTask<TValidator, TSender, TData>(this
            IServiceCollection serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TValidator>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TValidator, TSender, TData>(TaskType.ShadowTask, name, interval, data, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddShadowTask<TSelector, TSender, TData, TMsg>(this
            IServiceCollection serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TSelector>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TSelector, TSender, TData, TMsg>(TaskType.ShadowTask, name, interval, data, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddShadowTask<TSender, TData>(this
            IServiceCollection serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TSender>? implementationFactory = null
        )
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TSender, TData>(TaskType.ShadowTask, name, interval, data, implementationFactory);
    }

    public static IServiceTaskRegistry AddSystemTask<TValidator, TSelector, TSender, TData, TMsg>(this
            IServiceCollection serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TValidator>? validatorImplementationFactory = null,
            Func<IServiceProvider, TSelector>? selectorImplementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TValidator, TSelector, TSender, TData, TMsg>(TaskType.SystemTask, name, interval, data, validatorImplementationFactory, selectorImplementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddSystemTask<TValidator, TSender, TData>(this
            IServiceCollection serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TValidator>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TValidator, TSender, TData>(TaskType.SystemTask, name, interval, data, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddSystemTask<TSelector, TSender, TData, TMsg>(this
            IServiceCollection serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TSelector>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TSelector, TSender, TData, TMsg>(TaskType.SystemTask, name, interval, data, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddSystemTask<TSender, TData>(this
            IServiceCollection serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TSender>? implementationFactory = null
        )
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TSender, TData>(TaskType.SystemTask, name, interval, data, implementationFactory);
    }

    public static IServiceTaskRegistry AddUserTask<TValidator, TSelector, TSender, TData, TMsg>(this
            IServiceTaskRegistry serviceDescriptors,
            string? name = null,
            Func<IServiceProvider, TValidator>? validatorImplementationFactory = null,
            Func<IServiceProvider, TSelector>? selectorImplementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TValidator, TSelector, TSender, TData, TMsg>(TaskType.UserTask, name, null, null, validatorImplementationFactory, selectorImplementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddUserTask<TValidator, TSender, TData>(this
            IServiceTaskRegistry serviceDescriptors,
            string? name = null,
            Func<IServiceProvider, TValidator>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TValidator, TSender, TData>(TaskType.UserTask, name, null, null, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddUserTask<TSelector, TSender, TData, TMsg>(this
            IServiceTaskRegistry serviceDescriptors,
            string? name = null,
            Func<IServiceProvider, TSelector>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TSelector, TSender, TData, TMsg>(TaskType.UserTask, name, null, null, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddUserTask<TSender, TData>(this
            IServiceTaskRegistry serviceDescriptors,
            string? name = null,
            Func<IServiceProvider, TSender>? implementationFactory = null
        )
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TSender, TData>(TaskType.UserTask, name, null, null, implementationFactory);
    }

    public static IServiceTaskRegistry AddShadowTask<TValidator, TSelector, TSender, TData, TMsg>(this
            IServiceTaskRegistry serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TValidator>? validatorImplementationFactory = null,
            Func<IServiceProvider, TSelector>? selectorImplementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TValidator, TSelector, TSender, TData, TMsg>(TaskType.ShadowTask, name, interval, data, validatorImplementationFactory, selectorImplementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddShadowTask<TValidator, TSender, TData>(this
            IServiceTaskRegistry serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TValidator>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TValidator, TSender, TData>(TaskType.ShadowTask, name, interval, data, implementationFactory, senderImplementationFactory);
    }
    public static IServiceTaskRegistry AddShadowTask<TSelector, TSender, TData, TMsg>(this
            IServiceTaskRegistry serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TSelector>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TSelector, TSender, TData, TMsg>(TaskType.ShadowTask, name, interval, data, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddShadowTask<TSender, TData>(this
            IServiceTaskRegistry serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TSender>? implementationFactory = null
        )
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TSender, TData>(TaskType.ShadowTask, name, interval, data, implementationFactory);
    }

    public static IServiceTaskRegistry AddSystemTask<TValidator, TSelector, TSender, TData, TMsg>(this
            IServiceTaskRegistry serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TValidator>? validatorImplementationFactory = null,
            Func<IServiceProvider, TSelector>? selectorImplementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TValidator, TSelector, TSender, TData, TMsg>(TaskType.SystemTask, name, interval, data, validatorImplementationFactory, selectorImplementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddSystemTask<TValidator, TSender, TData>(this
            IServiceTaskRegistry serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TValidator>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TValidator : class, ITaskValidator<TData>
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TValidator, TSender, TData>(TaskType.SystemTask, name, interval, data, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddSystemTask<TSelector, TSender, TData, TMsg>(this
            IServiceTaskRegistry serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TSelector>? implementationFactory = null,
            Func<IServiceProvider, TSender>? senderImplementationFactory = null
        )
        where TSelector : class, IMessageSelector<TData, TMsg>
        where TSender : class, IMessageSender<TMsg>
        where TData : class
        where TMsg : class
    {
        return serviceDescriptors.AddTask<TSelector, TSender, TData, TMsg>(TaskType.SystemTask, name, interval, data, implementationFactory, senderImplementationFactory);
    }

    public static IServiceTaskRegistry AddSystemTask<TSender, TData>(this
            IServiceTaskRegistry serviceDescriptors,
            TimeSpan interval,
            TData data,
            string? name = null,
            Func<IServiceProvider, TSender>? implementationFactory = null
        )
        where TSender : class, IMessageSender<TData>
        where TData : class
    {
        return serviceDescriptors.AddTask<TSender, TData>(TaskType.SystemTask, name, interval, data, implementationFactory);
    }
}
