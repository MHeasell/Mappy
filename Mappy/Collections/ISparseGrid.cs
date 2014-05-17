namespace Mappy.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// Generic two-dimensional sparse grid interface.
    /// A sparse grid may contain holes, i.e. cells with no value.
    /// </summary>
    /// <typeparam name="T">The type of the grid cells.</typeparam>
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
