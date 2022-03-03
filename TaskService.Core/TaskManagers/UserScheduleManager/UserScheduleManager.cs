using TaskService.Core.Models;
using TaskService.Core.SchedulerWorkers.ScheduleManager;
using TaskService.Core.TaskManagers.Commands.UserScheduleManager;

namespace TaskService.Core.TaskManagers.UserScheduleManager;

/// <summary>
/// Создать задачу, реплан задачи, пауза, отмена, смена данных
/// </summary>
public class UserScheduleManager : BasePackOperation, IUserScheduleManager
{
    private const TaskType ManagerType = TaskType.UserTask;

    private readonly IScheduleManager _scheduleManager;

    public UserScheduleManager(IScheduleManager scheduleManager)
    {
        _scheduleManager = scheduleManager;
    }

    public Task<TaskKey> CreateTask(CreateTaskCommand createTaskCommand)
    {
        TimeSpan interval = createTaskCommand.Interval ?? TimeSpan.Zero;

        return _scheduleManager.ScheduleJobAsync(
            createTaskCommand.JobType,
            ManagerType,
            createTaskCommand.Data,
            createTaskCommand.StartAt,
            createTaskCommand.RepeatedCount,
            interval
        );
    }

    public Task<TaskKey> RescheduleTask(RescheduleTaskCommand rescheduleTask)
    {
        TimeSpan interval = rescheduleTask.Interval ?? TimeSpan.Zero;

        return _scheduleManager.RescheduleJobAsync(
            rescheduleTask.TriggerKey,
            ManagerType,
            rescheduleTask.StartAt,
            rescheduleTask.RepeatedCount,
            interval
        );
    }

    public Task<TaskKey> ChangeTaskPauseStatus(ChangeTaskPauseStatusCommand pauseStatusCommand)
    {
        return pauseStatusCommand.IsPause
            ? _scheduleManager.PauseTriggerAsync(pauseStatusCommand.TriggerKey, ManagerType)
            : _scheduleManager.UnPauseTriggerAsync(pauseStatusCommand.TriggerKey, ManagerType);
    }

    public Task<TaskKey> CancelTask(string triggerKey)
    {
        return _scheduleManager.CancelJobAsync(
            triggerKey,
            ManagerType
        );
    }

    public Task<TaskKey> ChangeTaskData(ChangeTaskDataCommand changeTaskData)
    {
        return _scheduleManager.ChangeTaskData(
            changeTaskData.TriggerKey,
            ManagerType,
            changeTaskData.Data
        );
    }

    public Task<IEnumerable<TaskKey>> CancelTask(IEnumerable<string> triggerKey)
    {
        return EntityPackOperationAsync(triggerKey, CancelTask);
    }

    public Task<IEnumerable<TaskKey>> ChangeTaskData(IEnumerable<ChangeTaskDataCommand> changeTaskData)
    {
        return EntityPackOperationAsync(changeTaskData, ChangeTaskData);
    }

    public Task<IEnumerable<TaskKey>> ChangeTaskPauseStatus(IEnumerable<ChangeTaskPauseStatusCommand> pauseStatusCommand)
    {
        return EntityPackOperationAsync(pauseStatusCommand, ChangeTaskPauseStatus);
    }

    public Task<IEnumerable<TaskKey>> CreateTask(IEnumerable<CreateTaskCommand> createTaskCommand)
    {
        return EntityPackOperationAsync(createTaskCommand, CreateTask);
    }

    public Task<IEnumerable<TaskKey>> RescheduleTask(IEnumerable<RescheduleTaskCommand> rescheduleTask)
    {
        return EntityPackOperationAsync(rescheduleTask, RescheduleTask);
    }
}
