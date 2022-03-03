namespace TaskService.Core.TaskExecutor.TaskExecutorImplementations;

public class TaskExecutorNullServiceException<TService> : Exception
{
    public TaskExecutorNullServiceException() : base(typeof(TService).FullName) { }
}
