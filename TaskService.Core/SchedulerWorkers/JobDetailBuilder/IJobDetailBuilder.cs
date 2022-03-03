using Quartz;

using TaskService.Core.Models;

namespace TaskService.Core.SchedulerWorkers.JobDetailBuilder;

public interface IJobDetailBuilder
{
    IJobDetail BuildJob(Type jobType, TaskType taskType, string name, string? comment = null);
    ITrigger BuildTrigger(TriggerKey triggerKey, JobKey jobKey, DateTime? startAt, uint? repeatCount, TimeSpan interval, IDictionary<string, string>? data = null);
}
