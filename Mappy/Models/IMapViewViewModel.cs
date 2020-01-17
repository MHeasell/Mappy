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

        void HandleMouseDownLeft(Point location, Keys modifierKeys);

        void MouseDownLeft(Point location);

        void MouseDownRight(MouseEventArgs e, Point location);

        void MouseDownShiftLeft(Point location);

        void MouseMove(Point locattion);

        void MouseUp();

        void KeyDown(Keys key);

        void LeaveFocus();

        void DragDrop(IDataObject item, Point location);

        void ClientSizeChanged(Size size);

        void ScrollPositionChanged(Point position);
    }
}
