using Nest;

namespace TaskService.Core.Models;

[ElasticsearchType(RelationName = nameof(ExecutorLog), IdProperty = nameof(Id))]
public record ExecutorLog(
        string Id,
        [property: Text(Fielddata = true)] string JobType,
        [property: Date(Name = "@timestamp")] DateTime StartAt,
        [property: Nested] IDictionary<string, string> Data,
        [property: Nested] JobSheduler JobSheduler,
        [property: Nested] TaskKey TaskKey
    ) : TimeseriesEntity(Id)
{
    [Ignore]
    public override string IndexKey => JobType;
};
