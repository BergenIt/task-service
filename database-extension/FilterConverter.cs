
using DatabaseExtension.Pagination;
using DatabaseExtension.Search;
using DatabaseExtension.Sort;
using DatabaseExtension.TimeRange;

using Google.Protobuf;

namespace DatabaseExtension;

public static class FilterConverter
{
    public static FilterContract FromProtoFilter(this Proto.Filter filter)
    {
        if (filter is null)
        {
            return new
            (
                new(null, null),
                Array.Empty<SearchFilter>(),
                Array.Empty<SortFilter>(),
                Array.Empty<TimeRangeFilter>()
            );
        }

        return new
        (
            filter.PaginationFilter.FromProtoPagination(),
            filter.SearchFilter.FromProtoSearch(),
            filter.SortFilter.FromProtoSort(),
            filter.TimeRangeFilter.FromProtoTimeRange()
        );
    }

    public static FilterContract FromProtoFilter<TS, TD>(this Proto.Filter filter) where TS : class, IMessage<TS> where TD : class
    {
        if (filter is null)
        {
            return new
            (
                new(null, null),
                Array.Empty<SearchFilter>(),
                Array.Empty<SortFilter>(),
                Array.Empty<TimeRangeFilter>()
            );
        }

        return new
        (
            filter.PaginationFilter.FromProtoPagination(),
            filter.SearchFilter.FromProtoSearch<TS, TD>(),
            filter.SortFilter.FromProtoSort<TS, TD>(),
            filter.TimeRangeFilter.FromProtoTimeRange<TS, TD>()
        );
    }
}

