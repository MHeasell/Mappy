namespace Mappy.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper for IGrid instances that provides an event
    /// for change notifications.
    /// </summary>
    /// <typeparam name="T">The type of the grid elements.</typeparam>
    public class BindingGrid<T> : IGrid<T>
    {
        private readonly IGrid<T> grid;

        public BindingGrid(IGrid<T> grid)
        {
            this.grid = grid;
        }

        public event EventHandler<GridEventArgs> CellsChanged;

        public int Width => this.grid.Width;

        public int Height => this.grid.Height;

        public T this[int index]
        {
            get
            {
                return this.grid[index];
            }

            set
            {
                this.grid[index] = value;
                var coords = this.grid.ToCoords(index);
                this.OnCellChanged(new GridEventArgs(coords.X, coords.Y));
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
                this.OnCellChanged(new GridEventArgs(x, y));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.grid.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected virtual void OnCellChanged(GridEventArgs args)
        {
            this.CellsChanged?.Invoke(this, args);
        }
    }
}
