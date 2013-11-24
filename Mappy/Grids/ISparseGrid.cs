namespace Mappy.Grids
{
    using System.Collections.Generic;

    public interface ISparseGrid<T> : IGrid<T>
    {
        IEnumerable<Entry<T>> Entries { get; }

        IEnumerable<T> Values { get; }

        bool HasValue(int x, int y);

        bool TryGetValue(int x, int y, out T val);

        bool Remove(int x, int y);

        void Merge(ISparseGrid<T> other, int x, int y);

        void Merge(ISparseGrid<T> other, int sourceX, int sourceY, int destX, int destY, int width, int height);
    }

    public struct Entry<T>
    {
        public int X;
        public int Y;
        public T Value;
    }
}
