
using Quartz;

namespace TaskService.Core.SchedulerWorkers;

public static class JobDataMapExtensions
{
    public static IDictionary<string, string> CreateDataMap(this JobDataMap data)
    {
        IDictionary<string, string> jsonData = data.ToDictionary(
           k => k.Key,
           v => v.Value.ToString() ?? string.Empty
        );

        return jsonData;
    }
}
