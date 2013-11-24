namespace Mappy.UI.Drawables
{
    using System.Drawing;

    public interface IDrawable
    {
        Size Size { get; }

        int Width { get; }

        int Height { get; }

        void Draw(Graphics graphics, Rectangle clipRectangle);
    }
}