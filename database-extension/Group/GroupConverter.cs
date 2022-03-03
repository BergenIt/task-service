using System.Reflection;

using DatabaseExtension.Config;

using Google.Protobuf;

namespace DatabaseExtension.Group;

public static class GroupConverter
{
    private static IDatabaseExtensionConfig s_databaseExtensionConfig = new DatabaseExtensionConfig();

    public static void InjectConfig(IDatabaseExtensionConfig databaseExtensionConfig)
    {
        s_databaseExtensionConfig = databaseExtensionConfig;
    }

    public static T ToProtoGroup<T>(this GroupData group) where T : class, IMessage<T>
    {
        T instance = Activator.CreateInstance<T>();

        GetProperties(instance, out PropertyInfo? column);

        if (column is null)
        {
            return instance;
        }

        column.SetValue(instance, group.GroupName);

        return instance;
    }
    public static T ToProtoGroup<T, TS, TD>(this GroupData group)
        where T : class, IMessage<T>
        where TS : class
        where TD : class, IMessage<TD>
    {
        T instance = Activator.CreateInstance<T>();

        GetProperties(instance, out PropertyInfo? column);

        if (column is null || group is null || string.IsNullOrEmpty(group.GroupName))
        {
            return instance;
        }

        column.SetValue(instance, s_databaseExtensionConfig.GetDistinationName<TS, TD>(group.GroupName));

        return instance;
    }
    public static GroupData FromProtoGroup<T>(this T sortProto) where T : class, IMessage<T>
    {
        GetProperties(sortProto, out PropertyInfo? column);

        if (column is null)
        {
            return new();
        }

        return new()
        {
            GroupName = (string?)column.GetValue(sortProto) ?? throw new NotImplementedException()
        };
    }

    public static GroupData FromProtoGroup<TS, TD>(this IMessage sortProto)
        where TS : class, IMessage<TS>
        where TD : class
    {
        GetProperties(sortProto, out PropertyInfo? column);
        string? columnName = (string?)column?.GetValue(sortProto);

        if (column is null || string.IsNullOrEmpty(columnName))
        {
            return new();
        }

        return new()
        {
            GroupName = s_databaseExtensionConfig.GetDistinationName<TS, TD>(columnName),
        };
    }

    private static void GetProperties(object instance, out PropertyInfo? column)
    {
        column = GetProp(instance, nameof(GroupData.GroupName));
    }

    private static PropertyInfo? GetProp(object instance, string name)
    {
        return instance.GetType().GetProperties().FirstOrDefault(p => p.Name == name);
    }
}
