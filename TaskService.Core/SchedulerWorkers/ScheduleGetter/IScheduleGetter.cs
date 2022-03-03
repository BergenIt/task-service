using DatabaseExtension;
using DatabaseExtension.Pagination;

using TaskService.Core.Models;

namespace TaskService.Core.SchedulerWorkers.ScheduleGetter;

public interface IScheduleGetter
{
    Task<IPageItems<ActiveTrigger>> GetActiveTriggers(TaskType taskType, FilterContract filter);

    Task<IPageItems<ErrorExecutorLog>> GetUserTaskLog(FilterContract filter);
    Task<IPageItems<JobDescriptor>> GetJobDescriptors(TaskType taskType, FilterContract filter);
}
