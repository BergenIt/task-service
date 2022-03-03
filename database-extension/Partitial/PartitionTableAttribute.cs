namespace DatabaseExtension.Partitial;

[AttributeUsage(AttributeTargets.Class)]
public class PartitionTableAttribute : Attribute
{
    private readonly string? _tableName;
    public PartitionTableAttribute() { }
    public PartitionTableAttribute(string tableName)
    {
        _tableName = tableName;
    }

    public static bool IsPartitionalClass<TEntity>()
    {
        return typeof(TEntity).CustomAttributes.Any(c => c.AttributeType == typeof(PartitionTableAttribute));
    }

    public static string GetTableName<TEntity>()
    {
        return ((PartitionTableAttribute?)typeof(TEntity)
            .GetCustomAttributes(typeof(PartitionTableAttribute), false)
            .SingleOrDefault())?._tableName
            ?? $"{typeof(TEntity).Name}s";
    }
}
