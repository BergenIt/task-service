namespace TaskService.Core.TaskExecutor.TaskExecutorImplementations;

public class SelectorException<TSelector, TData, TMsg> : Exception
{
    public SelectorException() : base($"Selector: {typeof(TSelector).FullName}.\nData: {typeof(TData).FullName}.\nMessage: {typeof(TMsg).FullName}") { }
}
