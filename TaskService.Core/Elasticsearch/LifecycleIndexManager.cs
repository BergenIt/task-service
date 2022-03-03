using DatabaseExtension;
using DatabaseExtension.Pagination;

using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models;
using TaskService.Core.Models.PolicyPhases;
using TaskService.Core.TaskRegistry;

namespace TaskService.Core.Elasticsearch;

public class LifecycleIndexManager : ILifecycleIndexManager
{
    private readonly IElasticsearchGetter _elasticsearchGetter;
    private readonly IElasticsearchWorker _elasticsearchWorker;

    private readonly IJobKeys _jobKeys;

    public LifecycleIndexManager(IElasticsearchGetter elasticsearchGetter, IElasticsearchWorker elasticsearchWorker, IJobKeys jobKeys)
    {
        _elasticsearchGetter = elasticsearchGetter;
        _elasticsearchWorker = elasticsearchWorker;
        _jobKeys = jobKeys;
    }

    public Task<IPageItems<PolicyPhases>> GetTaskPhases(FilterContract filterContract)
    {
        IEnumerable<string> keys = _jobKeys.TypeHumanNames.Keys;

        return _elasticsearchGetter.GetPolicyPhasesAsync<ExecutorLog>(keys, filterContract);
    }

    public Task UpdatePhases(IEnumerable<PolicyPhases> policyPhases)
    {
        return _elasticsearchWorker.UpdatePolicyPhases(policyPhases);
    }
}
