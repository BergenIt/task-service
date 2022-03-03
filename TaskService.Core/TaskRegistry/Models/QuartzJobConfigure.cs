
using Quartz;

namespace TaskService.Core.TaskRegistry;

public record QuartzJobConfigure(Type TaskType, Action<IJobConfigurator> Configurator);
