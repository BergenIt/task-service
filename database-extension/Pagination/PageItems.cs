
using System.Collections;

namespace DatabaseExtension.Pagination;

public interface IPageItems<T> : IReadOnlyCollection<T>
{
    long CountItems { get; }
    IEnumerable<T> Items { get; }
}

public record PageItems<T>(IEnumerable<T> Items, long CountItems) : IPageItems<T>
{
    public int Count => Items.Count();

    public IEnumerator<T> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Items.GetEnumerator();
    }
}
