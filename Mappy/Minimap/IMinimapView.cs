namespace Mappy.Minimap
{
    using System.Drawing;

    public interface IMinimapView
    {
        bool Visible { get; set; }

        Rectangle ViewportRectangle { get; set; }

        Image MinimapImage { get; set; }
    }
}
