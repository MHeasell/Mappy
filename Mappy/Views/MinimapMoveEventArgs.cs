namespace Mappy.Views
{
    using System;
    using System.Drawing;

    public class MinimapMoveEventArgs : EventArgs
    {
        public Point Location { get; set; }
    }
}