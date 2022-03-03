using Nest;

namespace TaskService.Core.Models;

[ElasticsearchType(RelationName = nameof(JobSheduler))]
public record JobSheduler(
    TimeSpan? Inverval,
    [property: Date] DateTimeOffset? NextFireTime,
    [property: Date] DateTimeOffset? PreviousFireTime,
    uint? Repeated
);
