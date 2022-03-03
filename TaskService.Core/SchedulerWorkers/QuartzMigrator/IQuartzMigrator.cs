
using Quartz;

using TaskService.Core.Models;
using TaskService.Core.TaskRegistry;

namespace TaskService.Core.SchedulerWorkers.QuartzMigrator;

public interface IQuartzMigrator
{
    Task Migrate(IDictionary<JobKey, BackTaskRegistry> jobsFromStart, WriteLevel level);
}
