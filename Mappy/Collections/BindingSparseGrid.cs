namespace Mappy.Collections
{
    using System;
    using System.Collections.Generic;

    public class BindingSparseGrid<T> : BindingGrid<T>, IBindingSparseGrid<T>
    {
        private readonly ISparseGrid<T> grid;

        public BindingSparseGrid(ISparseGrid<T> grid)
            : base(grid)
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

        public void Move(int oldIndex, int newIndex)
        {
            this.grid[newIndex] = this.grid[oldIndex];
            this.grid.Remove(oldIndex);
            this.OnEntryChanged(
                new SparseGridEventArgs(new[] { oldIndex }, new[] { newIndex }));
        }

        public void Move(int srcX, int srcY, int destX, int destY)
        {
            this.Move(this.ToIndex(srcX, srcY), this.ToIndex(destX, destY));
        }

        public bool HasValue(int x, int y)
        {
            return this.grid.HasValue(x, y);
        }

        public bool TryGetValue(int x, int y, out T val)
        {
            return this.grid.TryGetValue(x, y, out val);
        }

        public bool Remove(int x, int y)
        {
            if (this.grid.Remove(x, y))
            {
                base.OnCellChanged(x, y);
                var args = new SparseGridEventArgs(
                    SparseGridEventArgs.ActionType.Remove,
                    new[] { this.ToIndex(x, y) });
                this.OnEntryChanged(args);
                return true;
            }

            return false;
        }

        public bool Remove(int index)
        {
            var coords = this.ToCoords(index);
            return this.Remove(coords.X, coords.Y);
        }

        protected virtual void OnEntryChanged(SparseGridEventArgs args)
        {
            EventHandler<SparseGridEventArgs> h = this.EntriesChanged;
            if (h != null)
            {
                h(this, args);
            }
        }

        protected virtual void OnEntriesChanged(SparseGridEventArgs.ActionType type, IEnumerable<int> entries)
        {
            EventHandler<SparseGridEventArgs> h = this.EntriesChanged;
            if (h != null)
            {
                h(this, new SparseGridEventArgs(type, entries));
            }
        }

        protected override void OnCellChanged(int x, int y)
        {
            base.OnCellChanged(x, y);
            var args = new SparseGridEventArgs(
                SparseGridEventArgs.ActionType.Set,
                new[] { this.ToIndex(x, y) });
            this.OnEntryChanged(args);
        }

        protected override void OnAreaChanged(GridEventArgs args)
        {
            base.OnAreaChanged(args);
            this.OnEntriesChanged(
                SparseGridEventArgs.ActionType.Set,
                this.EnumerateCells(args.StartX, args.StartY, args.Width, args.Height));
        }

        private int ToIndex(int x, int y)
        {
            return (this.Width * y) + x;
        }

        private IEnumerable<int> EnumerateCells(int x, int y, int width, int height)
        {
            for (int dy = 0; dy < height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    yield return this.ToIndex(x + dx, y + dy);
                }
            }
        }
    }
}
