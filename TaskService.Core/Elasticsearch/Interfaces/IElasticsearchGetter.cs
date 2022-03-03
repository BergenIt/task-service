using DatabaseExtension;
using DatabaseExtension.Pagination;

using TaskService.Core.Models;
using TaskService.Core.Models.PolicyPhases;

namespace TaskService.Core.Elasticsearch.Interfaces;

public interface IElasticsearchGetter
{
    Task<IPageItems<TEntity>> GetEntities<TEntity>(FilterContract filterContract, string name = "") where TEntity : TimeseriesEntity;
    Task<IPageItems<PolicyPhases>> GetPolicyPhasesAsync<TTimeseriesEntity>(IEnumerable<string> keys, FilterContract filter) where TTimeseriesEntity : TimeseriesEntity;
}
