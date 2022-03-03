using DatabaseExtension;
using DatabaseExtension.Pagination;

using TaskService.Core.Models;
using TaskService.Core.SchedulerWorkers.ScheduleGetter;
using TaskService.Core.SchedulerWorkers.ScheduleManager;
using TaskService.Core.TaskManagers.SystemTaskManager.Commands;

namespace TaskService.Core.TaskManagers.SystemTaskManager;

public class SystemTaskManager : BasePackOperation, ISystemTaskManager
{
    private const TaskType ManagerType = TaskType.SystemTask;

    private readonly IScheduleManager _scheduleManager;
    private readonly IScheduleGetter _scheduleGetter;

    public SystemTaskManager(IScheduleManager scheduleManager, IScheduleGetter scheduleGetter)
    {
        _scheduleManager = scheduleManager;
        _scheduleGetter = scheduleGetter;
    }

    public Task<TaskKey> ChangeSystemTaskInterval(ChangeSystemTaskIntervalCommand intervalCommand)
    {
        return _scheduleManager.RescheduleJobAsync(intervalCommand.TriggerKey, ManagerType, null, null, intervalCommand.TimeSpan);
    }

    public Task<TaskKey> ChangeSystemTaskPauseStatus(ChangeSystemTaskPauseStatusCommand changeSystemTaskPauseStatusCommand)
    {
        return changeSystemTaskPauseStatusCommand.IsPause
            ? _scheduleManager.PauseTriggerAsync(changeSystemTaskPauseStatusCommand.TriggerKey, ManagerType)
            : _scheduleManager.UnPauseTriggerAsync(changeSystemTaskPauseStatusCommand.TriggerKey, ManagerType);
    }

    public async Task<IPageItems<SystemTask>> GetSystemTasks(FilterContract filter)
    {
        IPageItems<ActiveTrigger> commonActives = await _scheduleGetter.GetActiveTriggers(ManagerType, filter);

        PageItems<SystemTask> tasks = new(
            commonActives.Select(x => new SystemTask
            {
                Id = x.Id,
                Interval = x.Interval ?? TimeSpan.Zero,
                IsPaused = x.IsPaused,
                NextStart = x.NextStart,
                Name = x.JobType,
                LastStart = x.LastStart,
            }),
            commonActives.CountItems
        );

        return tasks;
    }

    public Task<IEnumerable<TaskKey>> ChangeSystemTaskInterval(IEnumerable<ChangeSystemTaskIntervalCommand> changeSystemTaskIntervals)
    {
        return EntityPackOperationAsync(changeSystemTaskIntervals, ChangeSystemTaskInterval);
    }

    public Task<IEnumerable<TaskKey>> ChangeSystemTaskPauseStatus(IEnumerable<ChangeSystemTaskPauseStatusCommand> changeSystemTaskPauseStatusCommands)
    {
        return EntityPackOperationAsync(changeSystemTaskPauseStatusCommands, ChangeSystemTaskPauseStatus);
    }
}
