
using TaskService.Core.Models;

namespace TaskService.Core.TaskExecutor;

public interface ITaskExecutor
{
    Task Execute(TaskMeta task);
}
