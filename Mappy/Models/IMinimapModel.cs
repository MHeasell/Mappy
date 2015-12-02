namespace Mappy.Models
{
    using System;
    using System.Drawing;

    public interface IMinimapModel
    {
        IObservable<int> MapWidth { get; }

        IObservable<int> MapHeight { get; }

        IObservable<bool> MinimapVisible { get; }

        IObservable<Bitmap> MinimapImage { get; }

        IObservable<Rectangle> MinimapRect { get; }
    }
}
