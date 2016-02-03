namespace Mappy.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Dense grid implementation backed by a one-dimensional array.
    /// </summary>
    /// <typeparam name="T">The type of the grid elements.</typeparam>
    [Serializable]
    public class Grid<T> : IGrid<T>
    {
        private readonly T[] arr;

        public Grid(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.arr = new T[width * height];
        }

        public int Width { get; }

        public int Height { get; }

        public T this[int index]
        {
            get
            {
                return this.arr[index];
            }

            set
            {
                this.arr[index] = value;
            }
        }

        public T this[int x, int y]
        {
            get
            {
                return this.arr[this.ToIndex(x, y)];
            }

            set
            {
                this.arr[this.ToIndex(x, y)] = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.arr.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.arr.GetEnumerator();
        }
    }
}
