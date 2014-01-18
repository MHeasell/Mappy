namespace Mappy.Collections
{
    using System.Collections.Generic;

    public interface IGrid<T> : IEnumerable<T>
    {
        int Width { get; }

        int Height { get; }

        T this[int index] { get; set; }

        T this[int x, int y] { get; set; }
    }
}
