
using Newtonsoft.Json;

using TaskService.Core.Models;
using TaskService.Core.TaskExecutor;

namespace TaskService.Core.TaskSystemImplementations.Http;

public class GrpcSelector : IMessageSelector<JobMergedData, string>
{
    private readonly IRouteParser<GrpcRoute> _routeParser;

    public GrpcSelector(IRouteParser<GrpcRoute> routeParser)
    {
        _routeParser = routeParser;
    }

    public Task<string> SelectMessage(string taskId, string taskType, JobMergedData data)
    {
        if (data.SelectorRoute is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        object response;

        using (GrpcRoute httpRoute = _routeParser.Parse(data.SelectorRoute))
        {
            Type requestType = httpRoute.MethodType.GetParameters()[0].ParameterType;

            object? request = JsonConvert.DeserializeObject(data.Data, requestType);

            if (request is null)
            {
                request = Activator.CreateInstance(requestType) ?? throw new NotImplementedException(requestType.FullName);
            }

            object? client = Activator.CreateInstance(httpRoute.ServiceCleintType, httpRoute.Channel) ?? throw new NotImplementedException(httpRoute.ServiceCleintType.FullName);

            response = httpRoute.MethodType.Invoke(client, new object[] { request }) ?? new { };
        }

        string result = JsonConvert.SerializeObject(response);

        return Task.FromResult(result);
    }
}
