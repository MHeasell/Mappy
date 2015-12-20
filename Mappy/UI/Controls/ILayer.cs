namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;

    public interface ILayer
    {
        event EventHandler<LayerChangedEventArgs> LayerChanged;

        void Draw(Graphics graphics, Rectangle clipRectangle);
    }
}