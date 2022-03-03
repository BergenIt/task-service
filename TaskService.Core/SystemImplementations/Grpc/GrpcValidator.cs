
using Newtonsoft.Json;

using TaskService.Core.Models;
using TaskService.Core.TaskExecutor;

namespace TaskService.Core.TaskSystemImplementations.Http;

public class GrpcValidator : ITaskValidator<JobMergedData>
{
    private readonly IRouteParser<GrpcRoute> _routeParser;

    public GrpcValidator(IRouteParser<GrpcRoute> routeParser)
    {
        _routeParser = routeParser;
    }

    public Task<bool> Validate(string taskId, string taskType, JobMergedData data)
    {
        if (data.ValidatorRoute is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        object response;

        using (GrpcRoute httpRoute = _routeParser.Parse(data.ValidatorRoute))
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

        ValidatorResponse httpResponseMessage = JsonConvert.DeserializeObject<ValidatorResponse>(result)
            ?? throw new InvalidCastException($"Not parse yaml from: {result}");

        return Task.FromResult(httpResponseMessage.IsValid);
    }
}
