using Quartz;

namespace TaskService.Core.Models;

public record TaskKey(string TriggerKey, TaskType TaskType)
{
    public static implicit operator TriggerKey(TaskKey taskKey)
    {
        return new(taskKey.TriggerKey, taskKey.TaskType.ToString());
    }

    public static implicit operator TaskKey(TriggerKey triggerKey)
    {
        return new(triggerKey.Name, Enum.Parse<TaskType>(triggerKey.Group));
    }
}
