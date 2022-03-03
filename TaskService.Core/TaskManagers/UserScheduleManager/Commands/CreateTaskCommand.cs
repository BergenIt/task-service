namespace TaskService.Core.TaskManagers.Commands.UserScheduleManager;

public class CreateTaskCommand
{
    public IDictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
    public string JobType { get; set; } = string.Empty;

    public DateTime StartAt { get; set; }
    public uint RepeatedCount { get; set; }
    public TimeSpan? Interval { get; set; }
}
