using DatabaseExtension;
using DatabaseExtension.Pagination;
using DatabaseExtension.Search;
using DatabaseExtension.Sort;
using DatabaseExtension.TimeRange;

using Nest;

using TaskService.Core.Elasticsearch;
using TaskService.Core.Elasticsearch.Interfaces;
using TaskService.Core.Models;
using TaskService.Core.Models.PolicyPhases;

namespace TaskService.Elasticsearch;

/// <summary>
/// Получить данные из эластика
/// </summary>
public class ElasticsearchGetter : IElasticsearchGetter
{
    private readonly IElasticClient _elasticClient;

    /// <summary>
    /// Получить данные из эластика
    /// </summary>
    /// <param name="elasticClient"></param>
    public ElasticsearchGetter(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    /// <summary>
    /// Получить политики жизненных циклов по ключам сущности
    /// </summary>
    /// <typeparam name="TTimeseriesEntity"></typeparam>
    /// <param name="keys"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public async Task<IPageItems<PolicyPhases>> GetPolicyPhasesAsync<TTimeseriesEntity>(IEnumerable<string> keys, FilterContract filter) where TTimeseriesEntity : TimeseriesEntity
    {
        IEnumerable<IGetLifecycleRequest> getLifecycleRequests = keys.Select(key => new GetLifecycleRequest($"{typeof(TTimeseriesEntity).Name}-{key}-policy".ToLowerInvariant()));

        IEnumerable<KeyValuePair<string, LifecyclePolicy>> lifecyclePolicies = await Task
            .WhenAll(getLifecycleRequests.Select(r => _elasticClient.IndexLifecycleManagement.GetLifecycleAsync(r)))
            .ContinueWith(t => t.Result.Select(r => r.Policies.Single()), TaskScheduler.Current);

        List<PolicyPhases> policyPhases = new();

        foreach (KeyValuePair<string, LifecyclePolicy> lifecyclePolicy in lifecyclePolicies)
        {
            IPhase hot = lifecyclePolicy.Value.Policy.Phases.Hot;
            RolloverLifecycleAction rolloverLifecycleAction = (RolloverLifecycleAction)hot.Actions.Single().Value;

            HotPolicyPhase hotPolicyPhase = new(
                rolloverLifecycleAction.MaximumAge.ToTimeSpan(),
                rolloverLifecycleAction.MaximumSize,
                rolloverLifecycleAction.MaximumDocuments
            );

            DeletePolicyPhase deletePolicyPhase = new(lifecyclePolicy.Value.Policy.Phases.Delete.MinimumAge.ToTimeSpan());

            WarmPolicyPhase warmPolicyPhase = new(lifecyclePolicy.Value.Policy.Phases.Warm.MinimumAge.ToTimeSpan());

            policyPhases.Add(new(lifecyclePolicy.Key, hotPolicyPhase, warmPolicyPhase, deletePolicyPhase));
        }

        IEnumerable<PolicyPhases> result = policyPhases.Search(filter.SearchFilters);

        int count = result.Count();

        PageItems<PolicyPhases> policyPhasesPage = new(result.Sort(filter.SortFilters).Paginations(filter.PaginationFilter), count);

        return policyPhasesPage;
    }

    /// <summary>
    /// Получить сущности с фильтром
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="filterContract"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IPageItems<TEntity>> GetEntities<TEntity>(FilterContract filterContract, string name = "") where TEntity : TimeseriesEntity
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(TEntity).Name;
        }

        string index = $"{name}-*".ToLowerInvariant();

        SearchRequest searchRequest = new(index)
        {
            PostFilter = new QueryContainer()
        };

        searchRequest = BuildSearchRequest(searchRequest, filterContract.SearchFilters, filterContract.TimeFilter);

        CountResponse countResponse = await _elasticClient.CountAsync(new CountRequest<TEntity>(index)
        {
            Query = searchRequest.PostFilter,
        });

        searchRequest = AddSortPageFilters(searchRequest, filterContract);

        ISearchResponse<TEntity> searchResponse = await _elasticClient.SearchAsync<TEntity>(searchRequest);

        if (countResponse.ServerError is not null || searchResponse.ServerError is not null)
        {
            throw new ElasticGetterException($"{countResponse.DebugInformation}\n{searchResponse.DebugInformation}");
        }

        IEnumerable<TEntity> items = searchResponse.Hits.Select(h => h.Source with { Id = h.Id });

        return new PageItems<TEntity>(items, countResponse.Count);
    }

    private static SearchRequest BuildSearchRequest(SearchRequest searchRequest, IEnumerable<SearchFilter> searches, IEnumerable<TimeRangeFilter> timeRangeFilters)
    {
        foreach (IGrouping<string, SearchFilter> searchFilterGroup in searches.GroupBy(s => s.ColumnName))
        {
            QueryContainer queryContainerProppery = new();

            foreach (SearchFilter searchFilter in searchFilterGroup)
            {
                string[] values = searchFilter.Value.Split(' ', StringSplitOptions.TrimEntries);

                if (!values.Any())
                {
                    break;
                }

                QueryContainer oneStringsStringQuery = new();

                foreach (string value in values)
                {
                    oneStringsStringQuery = oneStringsStringQuery && new QueryStringQuery()
                    {
                        Fields = new Field(searchFilterGroup.Key.ToLowerInvariant()),
                        Query = $"{searchFilter.Value}*",
                        AllowLeadingWildcard = true,
                        EnablePositionIncrements = true,
                    };
                }

                queryContainerProppery = queryContainerProppery || oneStringsStringQuery;
            }

            searchRequest.PostFilter = searchRequest.PostFilter && queryContainerProppery;
        }

        foreach (IGrouping<string, TimeRangeFilter> timeRangeFiltersGroup in timeRangeFilters.GroupBy(s => s.ColumnName))
        {
            QueryContainer queryContainerProppery = new();

            foreach (TimeRangeFilter filter in timeRangeFiltersGroup)
            {
                DateRangeQuery dateRangeQuery = GetDateTimeQuery(timeRangeFiltersGroup.Key, filter.StartRange, filter.EndRange);

                queryContainerProppery = queryContainerProppery || dateRangeQuery;
            }

            searchRequest.PostFilter = searchRequest.PostFilter && queryContainerProppery;
        }

        return searchRequest;
    }

    private static SearchRequest AddSortPageFilters(SearchRequest searchRequest, FilterContract filterContract)
    {
        searchRequest.Size = filterContract.PaginationFilter.PageSize;
        searchRequest.From = filterContract.PaginationFilter.PageSize * (filterContract.PaginationFilter.PageNumber - 1);

        searchRequest.Sort = filterContract.SortFilters.Select(s => (ISort)new FieldSort
        {
            Field = s.ColumnName.ToLowerInvariant(),
            Order = s.IsDescending ? SortOrder.Descending : SortOrder.Ascending,
        })
        .ToList();

        return searchRequest;
    }

    private static DateRangeQuery GetDateTimeQuery(string field, DateTime startDate, DateTime endDate)
    {
        if (startDate != DateTime.MinValue || endDate != DateTime.MinValue)
        {
            DateRangeQuery dateRangeQuery = new() { Field = new Field(field.ToLower()) };
            dateRangeQuery.GreaterThanOrEqualTo = startDate;
            dateRangeQuery.LessThanOrEqualTo = endDate.AddDays(1);

            return dateRangeQuery;
        }

        return new();
    }
}
