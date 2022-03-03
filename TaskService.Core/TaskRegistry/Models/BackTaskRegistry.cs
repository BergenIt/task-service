namespace TaskService.Core.TaskRegistry;

public record BackTaskRegistry(Type TaskType, string Schema, TimeSpan TimeSpan, IDictionary<string, string> Data, IDictionary<string, string> RootData)
    : TaskRegistryDescriptor(TaskType, Schema, RootData);

