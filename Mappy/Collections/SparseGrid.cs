namespace Mappy.Collections
{
    using System;
    using System.Collections.Generic;

    public class SparseGrid<T> : ISparseGrid<T>
    {
        private readonly IDictionary<int, T> values;

        public SparseGrid(int width, int height)
        {
            this.values = new Dictionary<int, T>();
            this.Width = width;
            this.Height = height;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public IEnumerable<Entry<T>> Entries
        {
            get
            {
                foreach (var i in this.values)
                {
                    Entry<T> e = new Entry<T>();
                    e.X = i.Key % this.Width;
                    e.Y = i.Key / this.Width;
                    e.Value = i.Value;
                    yield return e;
                }
            }
        }

        public IEnumerable<T> Values
        {
            get
            {
                return this.values.Values;
            }
        }

        public T this[int x, int y]
        {
            get
            {
                this.CheckIndexInBounds(x, y);

                T val;
                this.values.TryGetValue(this.ToIndex(x, y), out val);
                return val;
            }

            set
            {
                this.CheckIndexInBounds(x, y);

                if (EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    this.Remove(x, y);
                }
                else
                {
                    this.values[this.ToIndex(x, y)] = value;
                }
            }
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

        public IEnumerator<T> GetEnumerator()
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    yield return this[x, y];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool HasValue(int x, int y)
        {
            this.CheckIndexInBounds(x, y);
            return this.values.ContainsKey(this.ToIndex(x, y));
        }

        public bool TryGetValue(int x, int y, out T val)
        {
            this.CheckIndexInBounds(x, y);
            return this.values.TryGetValue(this.ToIndex(x, y), out val);
        }

        public bool Remove(int x, int y)
        {
            this.CheckIndexInBounds(x, y);
            return this.values.Remove(this.ToIndex(x, y));
        }

        /// <summary>
        /// Note: take care when merging large grids.
        /// A record is added for every element, even if its value is the default.
        /// This may lead to larger storage requirements than intended.
        /// Consider merging an ISparseGrid instead.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Merge(IGrid<T> other, int x, int y)
        {
            if (x < 0 || y < 0 || x + other.Width > this.Width || y + other.Height > this.Height)
            {
                throw new IndexOutOfRangeException("part of the other grid falls outside boundaries");
            }

            for (int dy = 0; dy < other.Height; dy++)
            {
                for (int dx = 0; dx < other.Width; dx++)
                {
                    T val = other[dx, dy];
                    this[x + dx, y + dy] = val;
                }
            }
        }

        public void Merge(ISparseGrid<T> other, int x, int y)
        {
            if (x < 0 || y < 0 || x + other.Width > this.Width || y + other.Height > this.Height)
            {
                throw new IndexOutOfRangeException("part of the other grid falls outside boundaries");
            }

            foreach (var e in other.Entries)
            {
                this[e.X + x, e.Y + y] = e.Value;
            }
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

        public void Merge(ISparseGrid<T> other, int sourceX, int sourceY, int destX, int destY, int width, int height)
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

            foreach (var e in other.Entries)
            {
                if (e.X < sourceX || e.Y < sourceY || e.X >= sourceX + width || e.Y >= sourceY + height)
                {
                    continue;
                }

                this[destX + (e.X - sourceX), destY + (e.Y - sourceY)] = e.Value;
            }
        }

        private void CheckIndexInBounds(int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.Width || y >= this.Height)
            {
                throw new IndexOutOfRangeException(string.Format("Coordinates ({0}, {1}) out of range", x, y));
            }
        }
    }
}
