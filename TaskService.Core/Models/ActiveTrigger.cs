namespace TaskService.Core.Models;

public class ActiveTrigger : BaseEntity
{
    public string JobType { get; set; } = string.Empty;

    public DateTime StartAt { get; set; }
    public TimeSpan? Interval { get; set; }

    public DateTime? NextStart { get; set; }
    public DateTime? LastStart { get; set; }

    public bool IsPaused { get; set; }
    public uint? Repeated { get; set; }

    public IDictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

    public TaskKey? TaskKey { get; set; }
}
