
using TaskService.Core.Models;

namespace TaskService.Core.AuditWriter;

public interface IAuditWriter
{
    ValueTask ExecuteJobRecord<TData>(string jobName, TData taskData);
    ValueTask UpsertJobRecord(AuditTaskAction action, TaskType taskType, string jobName, IDictionary<string, string> taskData);
}
