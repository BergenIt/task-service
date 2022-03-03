
using Elasticsearch.Net;

using Nest;

using TaskService.Core.Elasticsearch;
using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models;
using TaskService.Core.Models.PolicyPhases;

namespace TaskService.Elasticsearch;

/// <summary>
///  Запись в эластик
/// </summary>
public class ElasticsearchWorker : IElasticsearchWorker
{
    private readonly IElasticClient _elasticClient;

    /// <summary>
    /// Запись в эластик
    /// </summary>
    /// <param name="elasticClient"></param>
    public ElasticsearchWorker(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    /// <summary>
    /// Запись в эластик
    /// </summary>
    /// <typeparam name="TTimeseriesEntity"></typeparam>
    /// <param name="timeseriesEntity"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task InsertAsync<TTimeseriesEntity>(TTimeseriesEntity timeseriesEntity, string name = "") where TTimeseriesEntity : TimeseriesEntity
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(TTimeseriesEntity).Name;
        }

        string pattern = $"{name}-{timeseriesEntity.IndexKey}".ToLowerInvariant();

        SerializableData<TTimeseriesEntity> body = PostData.Serializable(timeseriesEntity);

        IndexResponse stringResponse = await _elasticClient
            .LowLevel
            .IndexAsync<IndexResponse>(
                pattern,
                body
            );

        if (stringResponse.ServerError is not null || stringResponse.ApiCall.HttpStatusCode is null)
        {
            Serilog.Log.Logger.ForContext<ElasticsearchWorker>().Error(stringResponse.DebugInformation);

            throw new ElasticWorkerException(stringResponse.ServerError?.ToString() ?? nameof(ElasticsearchWorker));
        }
    }

    /// <summary>
    /// Запись в эластик
    /// </summary>
    /// <typeparam name="TTimeseriesEntity"></typeparam>
    /// <param name="timeseriesEntities"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task InsertAsync<TTimeseriesEntity>(IEnumerable<TTimeseriesEntity> timeseriesEntities, string name = "") where TTimeseriesEntity : TimeseriesEntity
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(TTimeseriesEntity).Name;
        }

        foreach (IGrouping<string, TTimeseriesEntity> timeseriesEntity in timeseriesEntities.GroupBy(e => e.IndexKey))
        {
            string pattern = $"{name}{timeseriesEntity.Key}".ToLowerInvariant();

            StringResponse stringResponse = await _elasticClient.LowLevel.IndexAsync<StringResponse>(pattern, PostData.Serializable(timeseriesEntity.ToArray()));

            if (!stringResponse.Success)
            {
                throw new ElasticWorkerException(stringResponse.DebugInformation);
            }
        }
    }

    /// <summary>
    /// Обновить политику жизненого индекса цикла
    /// </summary>
    /// <param name="policyPhases"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task UpdatePolicyPhases(IEnumerable<PolicyPhases> policyPhases)
    {
        foreach (PolicyPhases policy in policyPhases)
        {
            PutLifecycleRequest putLifecycleRequest = new(policy.Id)
            {
                Policy = new Policy
                {
                    Phases = (Phases)policy
                }
            };

            PutLifecycleResponse putLifecycleResponse = await _elasticClient.IndexLifecycleManagement.PutLifecycleAsync(putLifecycleRequest);

            if (putLifecycleResponse.ServerError is not null || putLifecycleResponse.ApiCall.HttpStatusCode is null)
            {
                throw new ElasticWorkerException(putLifecycleResponse.DebugInformation.ToString());
            }
        }
    }
}
