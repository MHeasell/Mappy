namespace Mappy.Models
{
    using System.ComponentModel;
    using System.Drawing;

    public interface IMinimapModel : INotifyPropertyChanged
    {
        int MapWidth { get; }

        int MapHeight { get; }

        bool MinimapVisible { get; }

        Point ViewportLocation { get; }

        int ViewportWidth { get; }

        int ViewportHeight { get; }

        Bitmap MinimapImage { get; }

        void SetViewportLocation(Point location);

        void HideMinimap();
    }
}
