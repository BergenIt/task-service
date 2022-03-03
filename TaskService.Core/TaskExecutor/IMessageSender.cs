namespace TaskService.Core.TaskExecutor;

public interface IMessageSender<in TData>
    where TData : class
{
    Task Send(string taskId, string taskType, TData data);
}


