namespace Mappy.Collections
{
    using System.Collections.Generic;

    public interface IGrid<T> : IEnumerable<T>
    {
        int Width { get; }

        int Height { get; }

        T Get(int x, int y);

        void Set(int x, int y, T value);

        /// <summary>
        /// Returns the specified grid square to its default value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void Clear(int x, int y);

        /// <summary>
        /// Merge the given grid into this one,
        /// with the top-left corner of the other grid positioned
        /// at the given coordinates.
        /// </summary>
        /// <param name="other">the grid to merge in</param>
        /// <param name="x">the x coordinate of the top-left corner</param>
        /// <param name="y">the y coordinate of the top-left corner</param>
        void Merge(IGrid<T> other, int x, int y);

        void Merge(IGrid<T> other, int sourceX, int sourceY, int destX, int destY, int width, int height);
    }
}
