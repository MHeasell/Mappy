namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.UI.Controls;

    public interface IMapViewViewModel
    {
        IObservable<Size> CanvasSize { get; }

        IObservable<ILayer> ItemsLayer { get; }

        IObservable<ILayer> VoidLayer { get; }

        ILayer GuidesLayer { get; }

        ILayer GridLayer { get; }

        IObservable<Point> ViewportLocation { get; }

        void MouseLeftDown(Point location);

        void MouseRightDown(Point location);

        void MouseMove(Point locattion);

        void MouseUp();

        void KeyDown(Keys key);

        void LeaveFocus();

        void DragDrop(IDataObject item, Point location);

        void ClientSizeChanged(Size size);

        void ScrollPositionChanged(Point position);
    }
}
