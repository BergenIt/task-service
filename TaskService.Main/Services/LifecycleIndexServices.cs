using AutoMapper;

using DatabaseExtension;
using DatabaseExtension.Pagination;

using Grpc.Core;

using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models.PolicyPhases;
using TaskService.Proto;

namespace TaskService.Services;

/// <summary>
/// Сервис работы с настройками политик жизненных цикла эластика
/// </summary>
public class LifecycleIndexServices : TaskLifecycleIndexService.TaskLifecycleIndexServiceBase
{
    private readonly ILifecycleIndexManager _lifecycleIndexManager;
    private readonly IMapper _mapper;

    /// <summary>
    /// Сервис работы с настройками политик эластика
    /// </summary>
    /// <param name="lifecycleIndexManager"></param>
    /// <param name="mapper"></param>
    public LifecycleIndexServices(ILifecycleIndexManager lifecycleIndexManager, IMapper mapper)
    {
        _lifecycleIndexManager = lifecycleIndexManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Получить политики задач
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<LifecycleIndicePage> GetTaskLifecycleIndex(GetLifecycleIndexRequest request, ServerCallContext context)
    {
        FilterContract filterContract = request.Filter.FromProtoFilter<LifecycleIndex, PolicyPhases>();

        IPageItems<PolicyPhases> policyPhases = await _lifecycleIndexManager.GetTaskPhases(filterContract);

        return new()
        {
            LifecycleIndexList = { _mapper.Map<IEnumerable<LifecycleIndex>>(policyPhases.Items) },
            CountItems = (int)policyPhases.CountItems,
        };
    }

    /// <summary>
    /// Обновить политики
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<LifecycleIndices> UpdateLifecycleIndex(LifecycleIndices request, ServerCallContext context)
    {
        IEnumerable<PolicyPhases> requestPolicyPhases = _mapper.Map<IEnumerable<PolicyPhases>>(request.LifecycleIndexList);

        await _lifecycleIndexManager.UpdatePhases(requestPolicyPhases);

        return new()
        {
            LifecycleIndexList = { request.LifecycleIndexList }
        };
    }
}
