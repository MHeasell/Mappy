namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;

    using Mappy.UI.Painters;

    public sealed class GridLayer : AbstractLayer, IDisposable
    {
        private GridPainter painter;

        public GridLayer(int cellSize, Color color)
        {
            this.painter = new GridPainter(cellSize, new Pen(color));
        }

        public int CellSize
        {
            get
            {
                return this.painter.CellSize;
            }

            set
            {
                if (this.painter.CellSize != value)
                {
                    this.painter = new GridPainter(value, this.painter.Pen);
                    this.OnLayerChanged();
                }
            }
        }

        public Color Color
        {
            get
            {
                return this.painter.Pen.Color;
            }

            set
            {
                var oldPen = this.painter.Pen;
                if (oldPen.Color != value)
                {
                    this.painter = new GridPainter(this.painter.CellSize, new Pen(value));
                    oldPen.Dispose();
                    this.OnLayerChanged();
                }
            }
        }

        public void Dispose()
        {
            this.painter.Pen.Dispose();
        }

        protected override void DoDraw(Graphics graphics, Rectangle clipRectangle)
        {
            this.painter.Paint(graphics, clipRectangle);
        }
    }
}
