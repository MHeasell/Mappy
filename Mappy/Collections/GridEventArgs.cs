namespace Mappy.Collections
{
    using System;

    /// <summary>
    /// Arguments for grid cell events.
    /// </summary>
    public class GridEventArgs : EventArgs
    {
        public GridEventArgs(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }
    }
}