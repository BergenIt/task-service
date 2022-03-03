using TaskService.Core.Models;
using TaskService.Core.SchedulerWorkers.ScheduleManager;
using TaskService.Core.TaskManagers.Commands.FireAndForgetManager;

namespace TaskService.Core.TaskManagers.FireAndForgetManager;

/// <summary>
/// Зажигает таски
/// </summary>
public class FireAndForgetManager : BasePackOperation, IFireAndForgetManager
{
    private const TaskType ManagerType = TaskType.UserTask;

    public IScheduleManager _scheduleManager;

    public FireAndForgetManager(IScheduleManager scheduleManager)
    {
        _scheduleManager = scheduleManager;
    }

    public Task<TaskKey> FireAndForgetExistTask(string triggerKey)
    {
        return _scheduleManager.RescheduleJobAsync(
            triggerKey,
            ManagerType,
            null,
            0,
            TimeSpan.Zero
        );
    }

    public Task<TaskKey> FireAndForgetTask(FireAndForgetTaskCommand fireAndForget)
    {
        return _scheduleManager.ScheduleJobAsync(
            fireAndForget.JobType,
            ManagerType,
            fireAndForget.JobData,
            null,
            0,
            TimeSpan.Zero
        );
    }

    public Task<IEnumerable<TaskKey>> FireAndForgetExistTask(IEnumerable<string> triggerKeys)
    {
        return EntityPackOperationAsync(triggerKeys, FireAndForgetExistTask);
    }

    public Task<IEnumerable<TaskKey>> FireAndForgetTask(IEnumerable<FireAndForgetTaskCommand> fireAndForgets)
    {
        return EntityPackOperationAsync(fireAndForgets, FireAndForgetTask);
    }
}
