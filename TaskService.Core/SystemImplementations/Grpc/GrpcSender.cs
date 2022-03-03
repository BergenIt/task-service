
using Newtonsoft.Json;

using TaskService.Core.Models;
using TaskService.Core.TaskExecutor;

namespace TaskService.Core.TaskSystemImplementations.Http;

public class GrpcSender : IMessageSender<JobMergedData>
{
    private readonly IRouteParser<GrpcRoute> _routeParser;

    public GrpcSender(IRouteParser<GrpcRoute> routeParser)
    {
        _routeParser = routeParser;
    }

    public Task Send(string taskId, string taskType, JobMergedData data)
    {
        using GrpcRoute httpRoute = _routeParser.Parse(data.SenderRoute);

        Type requestType = httpRoute.MethodType.GetParameters()[0].ParameterType;

        object? request = JsonConvert.DeserializeObject(data.Data, requestType);

        if (request is null)
        {
            request = Activator.CreateInstance(requestType) ?? throw new NotImplementedException(requestType.FullName);
        }

        object? client = Activator.CreateInstance(httpRoute.ServiceCleintType, httpRoute.Channel) ?? throw new NotImplementedException(httpRoute.ServiceCleintType.FullName);

        httpRoute.MethodType.Invoke(client, new object[] { request, new Grpc.Core.CallOptions() });

        return Task.CompletedTask;
    }
}
