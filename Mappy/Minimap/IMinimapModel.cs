namespace Mappy.Minimap
{
    using System.ComponentModel;
    using System.Drawing;

    public interface IMinimapModel : INotifyPropertyChanged
    {
        bool MinimapVisible { get; set; }

        RectangleF ViewportRectangle { get; set; }

        Bitmap MinimapImage { get; }
    }
}
