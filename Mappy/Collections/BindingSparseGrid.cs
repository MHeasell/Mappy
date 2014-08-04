namespace Mappy.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper for ISparseGrid instances that provides an event
    /// for change notifications.
    /// </summary>
    /// <typeparam name="T">The type of the grid elements.</typeparam>
    public class BindingSparseGrid<T> : ISparseGrid<T>
    {
        private readonly ISparseGrid<T> grid;

        public BindingSparseGrid(ISparseGrid<T> grid)
        {
            this.grid = grid;
        }

        public event EventHandler<SparseGridEventArgs> EntriesChanged;

        public IEnumerable<KeyValuePair<int, T>> IndexEntries
        {
            get { return this.grid.IndexEntries; }
        }

        public IEnumerable<KeyValuePair<GridCoordinates, T>> CoordinateEntries
        {
            get
            {
                return this.grid.CoordinateEntries;
            }
        }

        public IEnumerable<T> Values
        {
            get { return this.grid.Values; }
        }

        public int Width
        {
            get
            {
                return this.grid.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.grid.Height;
            }
        }

        public T this[int index]
        {
            get
            {
                return this.grid[index];
            }

            set
            {
                this.grid[index] = value;
                this.OnEntryChanged(SparseGridEventArgs.Set(index));
            }
        }

        public T this[int x, int y]
        {
            get
            {
                return this.grid[x, y];
            }

            set
            {
                this.grid[x, y] = value;
                this.OnEntryChanged(SparseGridEventArgs.Set(this.ToIndex(x, y)));
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            this.grid[newIndex] = this.grid[oldIndex];
            this.grid.Remove(oldIndex);
            this.OnEntryChanged(
                SparseGridEventArgs.Move(oldIndex, newIndex));
        }

        public void Move(int srcX, int srcY, int destX, int destY)
        {
            this.Move(this.ToIndex(srcX, srcY), this.ToIndex(destX, destY));
        }

        public bool HasValue(int x, int y)
        {
            return this.grid.HasValue(x, y);
        }

        public bool HasValue(int index)
        {
            return this.grid.HasValue(index);
        }

        public bool TryGetValue(int x, int y, out T val)
        {
            return this.grid.TryGetValue(x, y, out val);
        }

        public bool TryGetValue(int index, out T val)
        {
            return this.grid.TryGetValue(index, out val);
        }

        public bool Remove(int x, int y)
        {
            if (this.grid.Remove(x, y))
            {
                this.OnEntryChanged(SparseGridEventArgs.Remove(this.ToIndex(x, y)));
                return true;
            }

            return false;
        }

        public bool Remove(int index)
        {
            if (this.grid.Remove(index))
            {
                this.OnEntryChanged(SparseGridEventArgs.Remove(index));
                return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.grid.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.grid).GetEnumerator();
        }

        protected virtual void OnEntryChanged(SparseGridEventArgs args)
        {
            EventHandler<SparseGridEventArgs> h = this.EntriesChanged;
            if (h != null)
            {
                h(this, args);
            }
        }
    }
}
