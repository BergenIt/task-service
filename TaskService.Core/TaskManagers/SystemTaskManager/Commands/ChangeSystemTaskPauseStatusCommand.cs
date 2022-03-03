namespace TaskService.Core.TaskManagers.SystemTaskManager.Commands;

public record ChangeSystemTaskPauseStatusCommand(string TriggerKey, bool IsPause);
