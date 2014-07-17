namespace Mappy.Minimap
{
    using System.Drawing;

    public interface IMinimapView
    {
        bool Visible { get; set; }

        RectangleF ViewportRectangle { get; set; }

        Image MinimapImage { get; set; }
    }
}
