namespace TaskService.Core.TaskManagers.Commands.UserScheduleManager;

public record ChangeTaskDataCommand(string TriggerKey, IDictionary<string, string> Data);
