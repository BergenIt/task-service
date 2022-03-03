namespace TaskService.Core.TaskManagers.Commands.FireAndForgetManager;

public class FireAndForgetTaskCommand
{
    public IDictionary<string, string> JobData { get; set; } = new Dictionary<string, string>();
    public string JobType { get; set; } = string.Empty;
}
