namespace TaskService.Core.TaskManagers.Commands.UserScheduleManager;

public record ChangeTaskPauseStatusCommand(string TriggerKey, bool IsPause);
