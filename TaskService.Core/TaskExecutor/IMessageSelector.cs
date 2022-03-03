namespace TaskService.Core.TaskExecutor;

public interface IMessageSelector<in TData, TMsg>
    where TMsg : class
    where TData : class
{
    Task<TMsg> SelectMessage(string taskId, string taskType, TData data);
}
