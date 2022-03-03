using Nest;

namespace TaskService.Core.Models;

[ElasticsearchType(RelationName = nameof(ExecutorLog), IdProperty = nameof(Id))]
public record ErrorExecutorLog(
    string Id,
    string JobType,
    DateTime StartAt,
    IDictionary<string, string> Data,
    JobSheduler JobSheduler,
    TaskKey TaskKey,
    [property: Text(Fielddata = true)] string Error
) : ExecutorLog(Id, JobType, StartAt, Data, JobSheduler, TaskKey);
