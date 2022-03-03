using TaskService.Core.Models;
using TaskService.Core.TaskManagers.Commands.FireAndForgetManager;

namespace TaskService.Core.TaskManagers.FireAndForgetManager;

public interface IFireAndForgetManager
{
    Task<TaskKey> FireAndForgetTask(FireAndForgetTaskCommand fireAndForget);
    Task<IEnumerable<TaskKey>> FireAndForgetTask(IEnumerable<FireAndForgetTaskCommand> fireAndForgets);

    Task<TaskKey> FireAndForgetExistTask(string triggerKey);
    Task<IEnumerable<TaskKey>> FireAndForgetExistTask(IEnumerable<string> triggerKeys);
}
