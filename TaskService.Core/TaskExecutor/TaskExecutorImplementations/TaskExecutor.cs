
using Newtonsoft.Json;

using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models;

namespace TaskService.Core.TaskExecutor.TaskExecutorImplementations;

public class TaskExecutor<TTaskValidator, TMessageSender, TData> : BaseTaskExecutor
    where TData : class
    where TTaskValidator : class, ITaskValidator<TData>
    where TMessageSender : class, IMessageSender<TData>
{
    public TaskExecutor(IServiceProvider serviceProvider, IElasticsearchWorker elasticsearchWorker) : base(serviceProvider, elasticsearchWorker) { }

    public override async Task Execute(TaskMeta task)
    {
        TData taskData = JsonConvert.DeserializeObject<TData>(task.MergedJson, _settings)
            ?? throw new ArgumentException($"Not parse value {task.MergedJson}", task.MergedJson);

        await InvokeValidator<TTaskValidator, TData>(task, taskData);

        await InvokeSender<TMessageSender, TData>(task, taskData);
    }
}

public class TaskExecutor<TMessageSender, TData> : BaseTaskExecutor
    where TData : class
    where TMessageSender : class, IMessageSender<TData>
{
    public TaskExecutor(IServiceProvider serviceProvider, IElasticsearchWorker elasticsearchWorker) : base(serviceProvider, elasticsearchWorker) { }

    public override async Task Execute(TaskMeta task)
    {
        TData taskData = JsonConvert.DeserializeObject<TData>(task.MergedJson, _settings)
            ?? throw new ArgumentException($"Not parse value {task.MergedJson}", task.MergedJson);

        await InvokeSender<TMessageSender, TData>(task, taskData);
    }
}

public class TaskExecutor<TMessageSelector, TMessageSender, TData, TMsg> : BaseTaskExecutor
    where TData : class
    where TMsg : class
    where TMessageSelector : class, IMessageSelector<TData, TMsg>
    where TMessageSender : class, IMessageSender<TMsg>
{
    public TaskExecutor(IServiceProvider serviceProvider, IElasticsearchWorker elasticsearchWorker) : base(serviceProvider, elasticsearchWorker) { }

    public override async Task Execute(TaskMeta task)
    {
        TData taskData = JsonConvert.DeserializeObject<TData>(task.MergedJson, _settings)
            ?? throw new ArgumentException($"Not parse value {task.MergedJson}", task.MergedJson);

        TMsg msg = await InvokeSelector<TMessageSelector, TData, TMsg>(task, taskData);

        await InvokeSender<TMessageSender, TMsg>(task, msg);
    }
}

public class TaskExecutor<TTaskValidator, TMessageSelector, TMessageSender, TData, TMsg> : BaseTaskExecutor
    where TData : class
    where TMsg : class
    where TTaskValidator : class, ITaskValidator<TData>
    where TMessageSelector : class, IMessageSelector<TData, TMsg>
    where TMessageSender : class, IMessageSender<TMsg>
{
    public TaskExecutor(IServiceProvider serviceProvider, IElasticsearchWorker elasticsearchWorker) : base(serviceProvider, elasticsearchWorker) { }

    public override async Task Execute(TaskMeta task)
    {
        TData taskData = JsonConvert.DeserializeObject<TData>(task.MergedJson, _settings)
            ?? throw new ArgumentException($"Not parse value {task.MergedJson}", task.MergedJson);

        await InvokeValidator<TTaskValidator, TData>(task, taskData);

        TMsg msg = await InvokeSelector<TMessageSelector, TData, TMsg>(task, taskData);

        await InvokeSender<TMessageSender, TMsg>(task, msg);
    }
}
