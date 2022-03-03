namespace TaskService.Core.AuditWriter;

public record AuditCreateCommand(string Action, string Message, string IpAddress, string UserName);
