namespace Mappy.Collections
{
    using System.Collections.Generic;

    public interface ISparseGrid<T> : IGrid<T>
    {
        IEnumerable<Entry<T>> Entries { get; }

        IEnumerable<T> Values { get; }

        bool HasValue(int x, int y);

        bool TryGetValue(int x, int y, out T val);

        bool Remove(int x, int y);
    }

    public struct Entry<T>
    {
        public int X;
        public int Y;
        public T Value;
    }
}
