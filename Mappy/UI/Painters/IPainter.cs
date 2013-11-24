namespace Mappy.UI.Painters
{
    using System.Drawing;

    public interface IPainter
    {
        void Paint(Graphics g, Rectangle clipRectangle);
    }
}
