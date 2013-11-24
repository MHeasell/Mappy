namespace Mappy.Grids
{
    using System;
    using System.Drawing;

    public interface IBindingGrid<T> : IGrid<T>
    {
        event EventHandler<GridEventArgs> CellsChanged;
    }

    public class GridEventArgs : EventArgs
    {
        public Rectangle Area { get; set; }
    }
}
