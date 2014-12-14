namespace Mappy.Minimap
{
    using System.ComponentModel;
    using System.Drawing;

    using Geometry;

    using Mappy.Data;

    public interface IMinimapModel : INotifyPropertyChanged
    {
        int MapWidth { get; }

        int MapHeight { get; }

        bool MinimapVisible { get; set; }

        Rectangle2D ViewportRectangle { get; }

        Bitmap MinimapImage { get; }

        void SetViewportCenterNormalized(double x, double y);
    }
}
