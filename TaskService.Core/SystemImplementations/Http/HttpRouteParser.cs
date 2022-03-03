using TaskService.Core.TaskExecutor;

namespace TaskService.Core.TaskSystemImplementations.Http;

public class HttpRouteParser : IRouteParser<HttpRoute>
{
    public HttpRoute Parse(string route)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<HttpRoute>(route)
            ?? throw new InvalidCastException($"Not parse yaml from: {route}");
    }
}

