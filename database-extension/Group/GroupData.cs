namespace DatabaseExtension.Group;

public class GroupData
{
    public string GroupName { get; set; } = string.Empty;
    public static IEnumerable<GroupData> FromStringArray(IEnumerable<string> groupRawColumn)
    {
        return groupRawColumn?.Any() == true && !groupRawColumn.Any(x => x is null) ?
            groupRawColumn.Select(s =>
            new GroupData()
            {
                GroupName = s.Trim(),
            }) :
            Array.Empty<GroupData>();
    }
}
