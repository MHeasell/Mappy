namespace Mappy.Collections
{
    using System;
    using System.Collections.Generic;

    public class BindingGrid<T> : IBindingGrid<T>
    {
        private readonly IGrid<T> grid;

        public BindingGrid(IGrid<T> grid)
        {
            this.grid = grid;
        }

        public event EventHandler<GridEventArgs> CellsChanged;

        public int Width
        {
            get { return this.grid.Width; }
        }

        public int Height
        {
            get { return this.grid.Height; }
        }

        public T Get(int x, int y)
        {
            return this.grid.Get(x, y);
        }

        public void Set(int x, int y, T value)
        {
            this.grid.Set(x, y, value);
            this.OnAreaChanged(new GridEventArgs(x, y));
        }

        public void Clear(int x, int y)
        {
            this.grid.Clear(x, y);
            this.OnAreaChanged(new GridEventArgs(x, y));
        }

        public void Merge(IGrid<T> other, int x, int y)
        {
            this.Merge(other, 0, 0, x, y, other.Width, other.Height);
        }

        public void Merge(IGrid<T> other, int sourceX, int sourceY, int destX, int destY, int width, int height)
        {
            this.grid.Merge(other, sourceX, sourceY, destX, destY, width, height);
            this.OnAreaChanged(new GridEventArgs(destX, destY, width, height));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.grid.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected virtual void OnCellChanged(int x, int y)
        {
            EventHandler<GridEventArgs> h = this.CellsChanged;
            if (h != null)
            {
                h(this, new GridEventArgs(x, y, 1, 1));
            }
        }

        protected virtual void OnAreaChanged(GridEventArgs args)
        {
            EventHandler<GridEventArgs> h = this.CellsChanged;
            if (h != null)
            {
                h(this, args);
            }
        }
    }
}
