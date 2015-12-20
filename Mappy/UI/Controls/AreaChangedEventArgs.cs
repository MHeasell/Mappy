namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;

    public class AreaChangedEventArgs : EventArgs
    {
        public AreaChangedEventArgs(Rectangle changedRectangle)
        {
            this.ChangedRectangle = changedRectangle;
        }

        public Rectangle ChangedRectangle { get; }
    }
}