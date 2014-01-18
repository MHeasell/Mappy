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

        private void CheckIndexInBounds(int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.Width || y >= this.Height)
            {
                throw new IndexOutOfRangeException(string.Format("Coordinates ({0}, {1}) out of range", x, y));
            }
        }
    }
}
