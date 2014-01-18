namespace Mappy.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Grid<T> : IGrid<T>
    {
        private readonly T[] arr;

        public Grid(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.arr = new T[width * height];
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

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

        public void Merge(IGrid<T> other, int x, int y)
        {
            this.Merge(other, 0, 0, x, y, other.Width, other.Height);
        }

        public void Merge(IGrid<T> other, int sourceX, int sourceY, int destX, int destY, int width, int height)
        {
            if (width < 0 || height < 0)
            {
                throw new ArgumentException("copy area has negative dimensions");
            }

            if (sourceX < 0 || sourceY < 0 || sourceX + width > other.Width || sourceY + height > other.Height)
            {
                throw new IndexOutOfRangeException("source area overlaps source bounds");
            }

            if (destX < 0 || destY < 0 || destX + width > this.Width || destY + height > this.Height)
            {
                throw new IndexOutOfRangeException("destination area overlaps destination bounds");
            }

            for (int dy = 0; dy < height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    this[destX + dx, destY + dy] = other[sourceX + dx, sourceY + dy];
                }
            }
        }
    }
}
