using TaskService.Core.Models;

namespace TaskService.Core.Elasticsearch.Interfaces;

public interface IElasticsearchMigrator
{
    Task MigrateDataStreamAsync<TTimeseriesEntity>(IEnumerable<string> keys) where TTimeseriesEntity : TimeseriesEntity;
    Task MigrateDataStreamAsync<TTimeseriesEntity>(string name, IEnumerable<string> keys) where TTimeseriesEntity : TimeseriesEntity;
}
