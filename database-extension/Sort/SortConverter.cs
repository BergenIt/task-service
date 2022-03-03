using DatabaseExtension.Config;

using Google.Protobuf;

namespace DatabaseExtension.Sort;

public static class SortConverter
{
    private static IDatabaseExtensionConfig s_databaseExtensionConfig = new DatabaseExtensionConfig();
    public static void InjectConfig(IDatabaseExtensionConfig databaseExtensionConfig)
    {
        s_databaseExtensionConfig = databaseExtensionConfig;
    }

    public static IEnumerable<Proto.SortFilter> ToProtoSort<TS, TD>(this IEnumerable<SortFilter> sort)
       where TS : class
       where TD : class, IMessage<TD>
    {
        return sort.Select(s => s.ToProtoSort<TS, TD>());
    }

    public static Proto.SortFilter ToProtoSort<TS, TD>(this SortFilter sort)
        where TS : class
        where TD : class, IMessage<TD>
    {
        Proto.SortFilter instance = new()
        {
            IsDescending = sort.IsDescending,
            ColumnName = s_databaseExtensionConfig.GetDistinationName<TS, TD>(sort.ColumnName)
        };

        return instance;
    }

    public static IEnumerable<Proto.SortFilter> ToProtoSort(this IEnumerable<SortFilter> sort)
    {
        return sort.Select(s => s.ToProtoSort());
    }

    public static Proto.SortFilter ToProtoSort(this SortFilter sort)
    {
        Proto.SortFilter instance = new()
        {
            IsDescending = sort.IsDescending,
            ColumnName = sort.ColumnName
        };

        return instance;
    }

    public static IEnumerable<SortFilter> FromProtoSort<TS, TD>(this IEnumerable<Proto.SortFilter> sortProto)
        where TS : class, IMessage<TS>
        where TD : class
    {
        return sortProto.Select(p => p.FromProtoSort<TS, TD>());
    }

    public static SortFilter FromProtoSort<TS, TD>(this Proto.SortFilter sortProto)
        where TS : class, IMessage<TS>
        where TD : class
    {
        string value = s_databaseExtensionConfig.GetDistinationName<TS, TD>(sortProto.ColumnName);

        return new(value, sortProto.IsDescending ?? false);
    }
    public static IEnumerable<SortFilter> FromProtoSort(this IEnumerable<Proto.SortFilter> sortProto)
    {
        return sortProto.Select(p => p.FromProtoSort());
    }

    public static SortFilter FromProtoSort(this Proto.SortFilter sortProto)
    {
        return new(sortProto.ColumnName, sortProto.IsDescending ?? false);
    }
}
