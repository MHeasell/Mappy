namespace Mappy.Views
{
    using System;
    using System.Drawing;
    using Models;

    public interface IMinimapView
    {
        event EventHandler<MinimapMoveEventArgs> ViewportMove;

        IMainView MainView { get; set; }

        IBindingMapModel Map { get; set; }
    }

    public class MinimapMoveEventArgs : EventArgs
    {
        public Point Location { get; set; }
    }
}
