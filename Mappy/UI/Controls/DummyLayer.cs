namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;

    public class DummyLayer : ILayer
    {
        public event EventHandler<LayerChangedEventArgs> LayerChanged;

        public void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            // do nothing
        }
    }
}
