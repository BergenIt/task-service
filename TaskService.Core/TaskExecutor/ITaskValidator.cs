namespace TaskService.Core.TaskExecutor;

public interface ITaskValidator<in TData>
    where TData : class
{
    Task<bool> Validate(string taskId, string taskType, TData data);
}
