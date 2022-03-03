namespace TaskService.Core.Models;

public class SystemTask : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public TimeSpan Interval { get; set; }

    public bool IsPaused { get; set; }

    public DateTime? NextStart { get; set; }
    public DateTime? LastStart { get; set; }
}
