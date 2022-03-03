using DatabaseExtension.Pagination;
using DatabaseExtension.Search;
using DatabaseExtension.Sort;
using DatabaseExtension.TimeRange;

namespace DatabaseExtension;

public record FilterContract(
    PaginationFilter PaginationFilter,
    IEnumerable<SearchFilter> SearchFilters,
    IEnumerable<SortFilter> SortFilters,
    IEnumerable<TimeRangeFilter> TimeFilter
);
