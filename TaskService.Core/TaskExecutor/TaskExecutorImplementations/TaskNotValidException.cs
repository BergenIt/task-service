namespace TaskService.Core.TaskExecutor.TaskExecutorImplementations;

public class TaskNotValidException<TValidator, TData> : Exception
{
    public TaskNotValidException() : base($"Validator: {typeof(TValidator).FullName}.\nData: {typeof(TData).FullName}") { }
}
