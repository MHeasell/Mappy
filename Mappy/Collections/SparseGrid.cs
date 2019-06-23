namespace Mappy.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Sparse grid implementation backed by a dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class SparseGrid<T> : ISparseGrid<T>
    {
        private readonly IDictionary<int, T> values;

        public SparseGrid(int width, int height)
        {
            this.values = new Dictionary<int, T>();
            this.Width = width;
            this.Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public IEnumerable<KeyValuePair<int, T>> IndexEntries => this.values;

        public IEnumerable<KeyValuePair<GridCoordinates, T>> CoordinateEntries
        {
            get
            {
                return this.values.Select(
                    x => new KeyValuePair<GridCoordinates, T>(
                        this.ToCoords(x.Key),
                        x.Value));
            }
        }

        public IEnumerable<T> Values => this.values.Values;

        public T this[int index]
        {
            get
            {
                this.CheckIndexInBounds(index);

                T val;
                this.values.TryGetValue(index, out val);
                return val;
            }

            set
            {
                this.CheckIndexInBounds(index);

                if (EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    this.Remove(index);
                }
                else
                {
                    this.values[index] = value;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var len = this.Width * this.Height;
            for (var i = 0; i < len; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool HasValue(int x, int y)
        {
            return this.HasValue(this.ToIndex(x, y));
        }

        public bool HasValue(int index)
        {
            this.CheckIndexInBounds(index);
            return this.values.ContainsKey(index);
        }

        public bool TryGetValue(int x, int y, out T val)
        {
            return this.TryGetValue(this.ToIndex(x, y), out val);
        }

        public bool TryGetValue(int index, out T val)
        {
            this.CheckIndexInBounds(index);
            return this.values.TryGetValue(index, out val);
        }

        public bool Remove(int x, int y)
        {
            return this.Remove(this.ToIndex(x, y));
        }

        public bool Remove(int index)
        {
            this.CheckIndexInBounds(index);
            return this.values.Remove(index);
        }

        private void CheckIndexInBounds(int index)
        {
            if (index < 0 || index >= this.Width * this.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}
