namespace TaskService.Core.TaskExecutor;

public interface IRouteParser<TAddress>
{
    TAddress Parse(string route);
}
