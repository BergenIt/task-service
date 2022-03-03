using TaskService.Core.Models;

namespace TaskService.Core.SchedulerWorkers.ScheduleManager;

public interface IScheduleManager
{
    Task<TaskKey> CancelJobAsync(string triggerKey, TaskType taskType);
    Task<TaskKey> PauseTriggerAsync(string triggerKey, TaskType taskType);
    Task<TaskKey> RescheduleJobAsync(string triggerKey, TaskType taskType, DateTime? startAt, uint? repeatCount, TimeSpan interval);
    Task<TaskKey> ScheduleJobAsync(string jobName, TaskType taskType, IDictionary<string, string> data, DateTime? startAt, uint? repeatCount, TimeSpan interval);
    Task<TaskKey> UnPauseTriggerAsync(string triggerKey, TaskType taskType);
    Task<TaskKey> ChangeTaskData(string triggerStrKey, TaskType taskType, IDictionary<string, string> inputData);
}
