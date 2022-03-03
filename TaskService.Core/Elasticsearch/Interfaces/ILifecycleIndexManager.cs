using DatabaseExtension;
using DatabaseExtension.Pagination;

using TaskService.Core.Models.PolicyPhases;

namespace TaskService.Core.Elasticsearch.Interfaces;

public interface ILifecycleIndexManager
{
    Task<IPageItems<PolicyPhases>> GetTaskPhases(FilterContract filterContract);
    Task UpdatePhases(IEnumerable<PolicyPhases> policyPhases);
}
