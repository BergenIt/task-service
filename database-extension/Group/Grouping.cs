namespace DatabaseExtension.Group;

public class Grouping<T> where T : class
{
    public string Key { get; set; } = string.Empty;
    public int Count => Group.Count();
    public IEnumerable<T> Group { get; set; } = Array.Empty<T>();
}
