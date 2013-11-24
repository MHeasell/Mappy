namespace Mappy.Grids
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class BindingSparseGrid<T> : BindingGrid<T>, IBindingSparseGrid<T>
    {
        private ISparseGrid<T> grid;

        public BindingSparseGrid(ISparseGrid<T> grid)
            : base(grid)
        {
            this.grid = grid;
        }

        public event EventHandler<SparseGridEventArgs> EntriesChanged;

        public IEnumerable<Entry<T>> Entries
        {
            get { return this.grid.Entries; }
        }

        public IEnumerable<T> Values
        {
            get { return this.grid.Values; }
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
                this.OnEntryChanged(SparseGridEventArgs.ActionType.Remove, x, y);
                return true;
            }

            return false;
        }

        public void Merge(ISparseGrid<T> other, int x, int y)
        {
            this.grid.Merge(other, x, y);
            this.OnEntriesChanged(SparseGridEventArgs.ActionType.Set, BindingSparseGrid<T>.ProcessEntries(other.Entries, x, y));
        }

        public void Merge(ISparseGrid<T> other, int sourceX, int sourceY, int destX, int destY, int width, int height)
        {
            throw new NotImplementedException("not currently supported");
        }

        protected virtual void OnEntryChanged(SparseGridEventArgs.ActionType type, int x, int y)
        {
            EventHandler<SparseGridEventArgs> h = this.EntriesChanged;
            if (h != null)
            {
                h(this, new SparseGridEventArgs { Action = type, Coordinates = new Point[] { new Point(x, y) } });
            }
        }

        protected virtual void OnEntriesChanged(SparseGridEventArgs.ActionType type, IEnumerable<Point> entries)
        {
            EventHandler<SparseGridEventArgs> h = this.EntriesChanged;
            if (h != null)
            {
                h(this, new SparseGridEventArgs { Action = type, Coordinates = entries });
            }
        }

        protected override void OnCellChanged(int x, int y)
        {
            base.OnCellChanged(x, y);
            this.OnEntryChanged(SparseGridEventArgs.ActionType.Set, x, y);
        }

        protected override void OnAreaChanged(Rectangle area)
        {
            base.OnAreaChanged(area);
            this.OnEntriesChanged(
                SparseGridEventArgs.ActionType.Set,
                BindingSparseGrid<T>.EnumerateRectCells(area));
        }

        private static IEnumerable<Point> EnumerateRectCells(Rectangle area)
        {
            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    yield return new Point(x + area.X, y + area.Y);
                }
            }
        }

        private static IEnumerable<Point> ProcessEntries(IEnumerable<Entry<T>> entries, int x, int y)
        {
            foreach (Entry<T> p in entries)
            {
                yield return new Point(p.X + x, p.Y + y);
            }
        }
    }
}
