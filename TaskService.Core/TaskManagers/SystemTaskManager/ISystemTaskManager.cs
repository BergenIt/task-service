using DatabaseExtension;
using DatabaseExtension.Pagination;

using TaskService.Core.Models;
using TaskService.Core.TaskManagers.SystemTaskManager.Commands;

namespace TaskService.Core.TaskManagers.SystemTaskManager;

public interface ISystemTaskManager
{
    Task<TaskKey> ChangeSystemTaskInterval(ChangeSystemTaskIntervalCommand intervalCommand);
    Task<IEnumerable<TaskKey>> ChangeSystemTaskInterval(IEnumerable<ChangeSystemTaskIntervalCommand> changeSystemTaskIntervals);
    Task<TaskKey> ChangeSystemTaskPauseStatus(ChangeSystemTaskPauseStatusCommand changeSystemTaskPauseStatusCommand);
    Task<IEnumerable<TaskKey>> ChangeSystemTaskPauseStatus(IEnumerable<ChangeSystemTaskPauseStatusCommand> changeSystemTaskPauseStatusCommands);
    Task<IPageItems<SystemTask>> GetSystemTasks(FilterContract filter);
}
