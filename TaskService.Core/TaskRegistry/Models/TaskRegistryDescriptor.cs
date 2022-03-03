namespace TaskService.Core.TaskRegistry;

public record TaskRegistryDescriptor(Type TaskType, string Schema, IDictionary<string, string> RootData);

