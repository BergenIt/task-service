namespace TaskService.Core.AuditWriter;

public enum AuditTaskAction
{
    Cancel,
    Rechedule,
    Pause,
    Unpause,
    DataChange,
    Create,
}
