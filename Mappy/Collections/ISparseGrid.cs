namespace Mappy.Collections
{
    using System.Collections.Generic;

    public interface ISparseGrid<T> : IGrid<T>
    {
        IEnumerable<KeyValuePair<int, T>> IndexEntries { get; }

        IEnumerable<KeyValuePair<GridCoordinates, T>> CoordinateEntries { get; }

        IEnumerable<T> Values { get; }

        bool HasValue(int x, int y);

        bool HasValue(int index);

        bool TryGetValue(int x, int y, out T val);

        bool TryGetValue(int index, out T val);

        bool Remove(int x, int y);

        bool Remove(int index);
    }
}
