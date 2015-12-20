namespace Mappy.UI.Drawables
{
    using System;
    using System.Drawing;

    using Mappy.UI.Controls;

    public interface IDrawable
    {
        event EventHandler<AreaChangedEventArgs> AreaChanged;

        Size Size { get; }

        int Width { get; }

        int Height { get; }

        void Draw(Graphics graphics, Rectangle clipRectangle);
    }
}