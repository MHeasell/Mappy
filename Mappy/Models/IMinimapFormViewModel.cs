namespace Mappy.Models
{
    using System;
    using System.Drawing;

    public interface IMinimapFormViewModel
    {
        IObservable<bool> MinimapVisible { get; }

        IObservable<Bitmap> MinimapImage { get; }

        IObservable<Rectangle> MinimapRect { get; }

        void MouseDown(Point location);

        void MouseMove(Point location);

        void MouseUp();

        void FormCloseButtonClick();
    }
}
