namespace TaskService.Core.TaskRegistry;

public record JobKeys(IDictionary<string, Type> TypeHumanNames) : IJobKeys;
