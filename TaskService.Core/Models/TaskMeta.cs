namespace TaskService.Core.Models;

public class TaskMeta : BaseEntity
{
    public string TaskType { get; set; } = string.Empty;
    public string MergedJson { get; set; } = string.Empty;
}

