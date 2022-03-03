namespace TaskService.Core.TaskManagers.Commands.UserScheduleManager;

public record RescheduleTaskCommand(string TriggerKey, DateTime StartAt, uint RepeatedCount = 0, TimeSpan? Interval = null);
