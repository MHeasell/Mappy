namespace Mappy.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Grid<T> : IGrid<T>
    {
        private readonly T[,] arr;

        public Grid(int width, int height)
        {
            this.arr = new T[height, width];
        }

        private Grid(T[,] arr)
        {
            this.arr = arr;
        }

        public int Width
        {
            get { return this.arr.GetLength(1); }
        }

        public int Height
        {
            get { return this.arr.GetLength(0); }
        }

        public T this[int index]
        {
            get
            {
                var coords = this.ToCoords(index);
                return this[coords.X, coords.Y];
            }

            set
            {
                var coords = this.ToCoords(index);
                this[coords.X, coords.Y] = value;
            }
        }

        public T this[int x, int y]
        {
            get
            {
                return this.arr[y, x];
            }

            set
            {
                this.arr[y, x] = value;
            }
        }

        public static Grid<T> GetView(T[,] arr)
        {
            return new Grid<T>(arr);
        }

        public T Get(int x, int y)
        {
            return this[x, y];
        }

        public void Set(int x, int y, T value)
        {
            this[x, y] = value;
        }

        public void Clear(int x, int y)
        {
            this.arr[y, x] = default(T);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.arr.Cast<T>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
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

        private int ToIndex(int x, int y)
        {
            return (y * this.Width) + x;
        }

        private Coords ToCoords(int index)
        {
            return new Coords(index % this.Width, index / this.Width);
        }

        private struct Coords
        {
            public Coords(int x, int y)
                : this()
            {
                this.X = x;
                this.Y = y;
            }

            public int X { get; set; }

            public int Y { get; set; }
        }
    }
}
