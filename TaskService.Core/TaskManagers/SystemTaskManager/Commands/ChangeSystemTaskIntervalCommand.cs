namespace TaskService.Core.TaskManagers.SystemTaskManager.Commands;

public record ChangeSystemTaskIntervalCommand(string TriggerKey, TimeSpan TimeSpan);
