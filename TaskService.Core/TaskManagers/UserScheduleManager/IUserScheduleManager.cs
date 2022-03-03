using TaskService.Core.Models;
using TaskService.Core.TaskManagers.Commands.UserScheduleManager;

namespace TaskService.Core.TaskManagers.UserScheduleManager;

public interface IUserScheduleManager
{
    Task<TaskKey> CancelTask(string triggerKey);
    Task<TaskKey> ChangeTaskData(ChangeTaskDataCommand changeTaskData);
    Task<TaskKey> ChangeTaskPauseStatus(ChangeTaskPauseStatusCommand pauseStatusCommand);
    Task<TaskKey> CreateTask(CreateTaskCommand createTaskCommand);
    Task<TaskKey> RescheduleTask(RescheduleTaskCommand rescheduleTask);

    Task<IEnumerable<TaskKey>> CancelTask(IEnumerable<string> triggerKey);
    Task<IEnumerable<TaskKey>> ChangeTaskData(IEnumerable<ChangeTaskDataCommand> changeTaskData);
    Task<IEnumerable<TaskKey>> ChangeTaskPauseStatus(IEnumerable<ChangeTaskPauseStatusCommand> pauseStatusCommand);
    Task<IEnumerable<TaskKey>> CreateTask(IEnumerable<CreateTaskCommand> createTaskCommand);
    Task<IEnumerable<TaskKey>> RescheduleTask(IEnumerable<RescheduleTaskCommand> rescheduleTask);
}
