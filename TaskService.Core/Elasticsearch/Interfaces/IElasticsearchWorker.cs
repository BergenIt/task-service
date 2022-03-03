
using TaskService.Core.Models;
using TaskService.Core.Models.PolicyPhases;

namespace TaskService.Core.Elasticsearch.Interfaces;

public interface IElasticsearchWorker
{
    Task InsertAsync<TTimeseriesEntity>(IEnumerable<TTimeseriesEntity> timeseriesEntities, string name = "") where TTimeseriesEntity : TimeseriesEntity;
    Task InsertAsync<TTimeseriesEntity>(TTimeseriesEntity timeseriesEntity, string name = "") where TTimeseriesEntity : TimeseriesEntity;
    Task UpdatePolicyPhases(IEnumerable<PolicyPhases> policyPhases);
}
