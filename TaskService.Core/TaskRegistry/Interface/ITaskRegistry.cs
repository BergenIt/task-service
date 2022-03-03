namespace TaskService.Core.TaskRegistry;

public interface ITaskRegistry
{
    IDictionary<string, BackTaskRegistry> ShadowJobs { get; }
    IDictionary<string, BackTaskRegistry> SystemJobs { get; }

    IDictionary<string, TaskRegistryDescriptor> UserJobs { get; }
}

public interface IJobKeys
{
    IDictionary<string, Type> TypeHumanNames { get; }
}

