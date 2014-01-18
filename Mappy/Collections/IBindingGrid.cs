namespace Mappy.Collections
{
    using System;

    public interface IBindingGrid<T> : IGrid<T>
    {
        event EventHandler<GridEventArgs> CellsChanged;
    }

    public class GridEventArgs : EventArgs
    {
        public GridEventArgs(int x, int y)
            : this(x, y, 1, 1)
        {
        }

        public GridEventArgs(int startX, int startY, int width, int height)
        {
            this.StartX = startX;
            this.StartY = startY;
            this.Width = width;
            this.Height = height;
        }

        public int StartX { get; set; }

        public int StartY { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
