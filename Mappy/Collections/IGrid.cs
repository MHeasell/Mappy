namespace Mappy.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// Generic two-dimensional dense grid interface.
    /// </summary>
    /// <typeparam name="T">The type of the grid cells.</typeparam>
    public interface IGrid<T> : IEnumerable<T>
    {
        int Width { get; }

        int Height { get; }

        T this[int index] { get; set; }
    }
}
