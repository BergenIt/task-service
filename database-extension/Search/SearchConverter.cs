using DatabaseExtension.Config;

using Google.Protobuf;

namespace DatabaseExtension.Search;

public static class SearchConverter
{
    private static IDatabaseExtensionConfig s_databaseExtensionConfig = new DatabaseExtensionConfig();

    public static void InjectConfig(IDatabaseExtensionConfig databaseExtensionConfig)
    {
        s_databaseExtensionConfig = databaseExtensionConfig;
    }

    public static IEnumerable<Proto.SearchFilter> ToProtoSearch<TS, TD>(this IEnumerable<SearchFilter> Search)
        where TS : class
        where TD : class, IMessage<TD>
    {
        return Search.Select(s => s.ToProtoSearch<TS, TD>());
    }

    public static Proto.SearchFilter ToProtoSearch<TS, TD>(this SearchFilter searchData)
        where TS : class
        where TD : class, IMessage<TD>
    {
        Proto.SearchFilter instance = new()
        {
            Value = searchData.Value,
            ColumnName = s_databaseExtensionConfig.GetDistinationName<TS, TD>(searchData.ColumnName)
        };

        return instance;
    }

    public static IEnumerable<Proto.SearchFilter> ToProtoSearch(this IEnumerable<SearchFilter> Search)
    {
        return Search.Select(s => s.ToProtoSearch());
    }

    public static Proto.SearchFilter ToProtoSearch(this SearchFilter searchData)
    {
        Proto.SearchFilter instance = new()
        {
            ColumnName = searchData.ColumnName,
            Value = searchData.Value,
        };

        return instance;
    }

    public static IEnumerable<SearchFilter> FromProtoSearch<TS, TD>(this IEnumerable<Proto.SearchFilter> searchProto)
        where TS : class, IMessage<TS>
        where TD : class
    {
        List<SearchFilter> result = new();

        IEnumerable<IGrouping<string, Proto.SearchFilter>> searchFilters = searchProto.GroupBy(s => s.ColumnName);

        foreach (IGrouping<string, Proto.SearchFilter> searchFilter in searchFilters)
        {
            IEnumerable<SearchFilter> searchFilterGroups = searchFilter.Select(f => f.FromProtoSearch<TS, TD>());

            string value = string.Join(SearchExtensions.Splitter, searchFilterGroups.Select(f => f.Value));

            SearchFilter searchFilterResult = new(searchFilterGroups.First().ColumnName, value);

            result.Add(searchFilterResult);
        }

        return result;
    }

    public static SearchFilter FromProtoSearch<TS, TD>(this Proto.SearchFilter searchProto)
        where TS : class, IMessage<TS>
        where TD : class
    {
        string column = s_databaseExtensionConfig.GetDistinationName<TS, TD>(searchProto.ColumnName);
        string value = s_databaseExtensionConfig.GetDistinationValue<TD>(searchProto.ColumnName, searchProto.Value) ?? searchProto.Value;

        return new(column, value);
    }

    public static IEnumerable<SearchFilter> FromProtoSearch(this IEnumerable<Proto.SearchFilter> searchProto)
    {
        List<SearchFilter> result = new();

        IEnumerable<IGrouping<string, Proto.SearchFilter>> searchFilters = searchProto
            .GroupBy(s => s.ColumnName);

        foreach (IGrouping<string, Proto.SearchFilter> searchFilter in searchFilters)
        {
            IEnumerable<SearchFilter> searchFilterGroups = searchFilter.Select(f => f.FromProtoSearch());

            string value = string.Join(SearchExtensions.Splitter, searchFilterGroups.Select(f => f.Value));

            SearchFilter searchFilterResult = new(searchFilterGroups.First().ColumnName, value);

            result.Add(searchFilterResult);
        }

        return result;
    }

    public static SearchFilter FromProtoSearch(this Proto.SearchFilter searchProto)
    {
        return new(searchProto.ColumnName, searchProto.Value);
    }
}

