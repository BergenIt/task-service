using System.Text;

using TaskService.Core.Models;
using TaskService.Core.TaskExecutor;

namespace TaskService.Core.TaskSystemImplementations.Http;

public class HttpSelector : IMessageSelector<JobMergedData, string>
{
    private readonly IRouteParser<HttpRoute> _routeParser;

    public HttpSelector(IRouteParser<HttpRoute> routeParser)
    {
        _routeParser = routeParser;
    }

    public async Task<string> SelectMessage(string taskId, string taskType, JobMergedData data)
    {
        if (data.SelectorRoute is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        HttpRoute httpRoute = _routeParser.Parse(data.SelectorRoute);

        using HttpClient httpClient = new();

        StringContent stringContent = new(data.Data, Encoding.UTF8, "application/json");

        Serilog.Log.Logger.Information($"Request content: {await stringContent.ReadAsStringAsync()}");

        HttpResponseMessage response = await httpClient.PostAsync(httpRoute.Uri, stringContent);

        Serilog.Log.Logger.Information($"Request meta: {response.RequestMessage}");
        Serilog.Log.Logger.Information($"Response: {response}");

        if (response.StatusCode is not System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException(
                $"Request to {httpRoute.Uri} with data {data.Data} failed. Response body {await response.Content.ReadAsStringAsync()}",
                null,
                response.StatusCode
            );
        }

        string jsonData = await response.Content.ReadAsStringAsync();

        return jsonData;
    }
}
