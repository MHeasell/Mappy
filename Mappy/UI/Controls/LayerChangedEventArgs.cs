namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;

    public class LayerChangedEventArgs : EventArgs
    {
        public LayerChangedEventArgs()
        {
            this.AllChanged = true;
            this.ChangedRectangle = Rectangle.Empty;
        }

        public LayerChangedEventArgs(Rectangle changedRectangle)
        {
            this.AllChanged = false;
            this.ChangedRectangle = changedRectangle;
        }

        public bool AllChanged { get; }

        public Rectangle ChangedRectangle { get; }
    }
}