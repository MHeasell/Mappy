namespace Mappy.UI.Controls
{
    using System.Drawing;

    using Mappy.UI.Painters;

    public class PainterLayer : AbstractLayer
    {
        private readonly IPainter painter;

        public PainterLayer(IPainter painter)
        {
            this.painter = painter;
        }

        public void Invalidate()
        {
            this.OnLayerChanged(new LayerChangedEventArgs());
        }

        protected override void DoDraw(Graphics graphics, Rectangle clipRectangle)
        {
            this.painter.Paint(graphics, clipRectangle);
        }
    }
}
