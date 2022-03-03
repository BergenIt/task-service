
using Elasticsearch.Net;

using Nest;

using Newtonsoft.Json;

using TaskService.Core.Elasticsearch;
using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models;
using TaskService.Core.Models.PutTemplateSettings;

namespace TaskService.Elasticsearch;

/// <summary>
/// Создает политики и шаблоны индексов
/// </summary>
public class ElasticsearchMigrator : IElasticsearchMigrator
{
    private readonly JsonSerializerSettings _settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore
    };

    private readonly IElasticClient _elasticClient;

    /// <summary>
    /// Создает политики и шаблоны индексов
    /// </summary>
    /// <param name="elasticClient"></param>
    public ElasticsearchMigrator(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    /// <summary>
    /// Создать политики и шаблоны индексов
    /// </summary>
    /// <typeparam name="TTimeseriesEntity"></typeparam>
    /// <param name="name"></param>
    /// <param name="keys"></param>
    /// <returns></returns>
    public async Task MigrateDataStreamAsync<TTimeseriesEntity>(string name, IEnumerable<string> keys) where TTimeseriesEntity : TimeseriesEntity
    {
        foreach (string key in keys)
        {
            string pattern = $"{name}-{key}".ToLowerInvariant();
            string policy = $"{pattern}-policy";
            string template = $"{pattern}-template";

            await PutLifecycleAsync(policy);

            ITypeMapping indexTemplate = new TypeMappingDescriptor<TTimeseriesEntity>().AutoMap<TTimeseriesEntity>();

            PutTemplateV2ForAllData value = new()
            {
                IndexPatterns = new string[] { pattern },
                DataStream = new { },
                Template = new()
                {
                    Settings = new()
                    {
                        NumberOfShards = 1,
                        NumberOfReplicas = 1,
                        IndexLifecycleName = policy
                    },
                    TypeMapping = _elasticClient.RequestResponseSerializer.SerializeToString(indexTemplate),
                },
            };

            string body = JsonConvert.SerializeObject(value, _settings)
                .Replace("\\", string.Empty)
                .Replace("\"{", "{")
                .Replace("}\"", "}");

            StringResponse stringResponse = await _elasticClient.LowLevel.Indices.PutTemplateV2ForAllAsync<StringResponse>(template, body);

            if (!stringResponse.Success)
            {
                throw new ElasticMigrateException(stringResponse.DebugInformation);
            }
        }
    }

    /// <summary>
    /// Создать политики и шаблоны индексов
    /// </summary>
    /// <typeparam name="TTimeseriesEntity"></typeparam>
    /// <param name="keys"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Task MigrateDataStreamAsync<TTimeseriesEntity>(IEnumerable<string> keys) where TTimeseriesEntity : TimeseriesEntity
    {
        string name = typeof(TTimeseriesEntity).Name;

        return MigrateDataStreamAsync<TTimeseriesEntity>(name, keys);
    }

    private async Task PutLifecycleAsync(string policy)
    {
        GetLifecycleResponse getLifecycleResponse = await _elasticClient
            .IndexLifecycleManagement
            .GetLifecycleAsync(l => l.PolicyId(policy));

        if (getLifecycleResponse?.Policies.Any() == true)
        {
            return;
        }

        LifecycleActions hotLifecycleActions = new();
        RolloverLifecycleAction hotRolloverLifecycleAction = new()
        {
            MaximumAge = new Time(new TimeSpan(days: 2, 0, 0, 0)),
            MaximumSize = "50GB",
        };
        hotLifecycleActions.Add(hotRolloverLifecycleAction);

        LifecycleActions deleteLifecycleActions = new();
        DeleteLifecycleAction deleteLifecycleAction = new();
        deleteLifecycleActions.Add(deleteLifecycleAction);

        PutLifecycleRequest putLifecycleRequest = new(policy)
        {
            Policy = new Policy
            {
                Phases = new Phases
                {
                    Hot = new Phase
                    {
                        MinimumAge = new Time(0),
                        Actions = hotLifecycleActions,
                    },
                    Warm = new Phase
                    {
                        MinimumAge = new Time(new TimeSpan(days: 30, 1, 0, 0)),
                        Actions = new LifecycleActions(),
                    },
                    Delete = new Phase
                    {
                        MinimumAge = new Time(new TimeSpan(days: 60, 0, 0, 0)),
                        Actions = deleteLifecycleActions,
                    },
                }
            }
        };

        PutLifecycleResponse putLifecycleResponse = await _elasticClient.IndexLifecycleManagement.PutLifecycleAsync(putLifecycleRequest);

        if (putLifecycleResponse.ServerError is not null)
        {
            throw new ElasticMigrateException(putLifecycleResponse.ServerError.ToString());
        }
    }
}
