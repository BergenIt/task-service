namespace TaskService.Core.AuditWriter;

public interface IJwtParser
{
    string GetUserName();
    string GetIpAddress();
}
