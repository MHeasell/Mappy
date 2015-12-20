namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public sealed class LayerView : ScrollableControl
    {
        private readonly LayerCollection layers = new LayerCollection();

        private Size canvasSize;

        public LayerView()
        {
            this.DoubleBuffered = true;
            this.layers.FullRedraw += this.LayersOnFullRedraw;
            this.layers.AreaChanged += this.LayersOnAreaChanged;
        }

        public event EventHandler CanvasSizeChanged;

        public LayerCollection Layers => this.layers;

        public Size CanvasSize
        {
            get
            {
                return this.canvasSize;
            }

            set
            {
                if (value != this.canvasSize)
                {
                    this.canvasSize = value;
                    this.OnCanvasSizeChanged();
                }
            }
        }

        public Point ToVirtualPoint(Point clientPoint)
        {
            return new Point(
                clientPoint.X - this.AutoScrollPosition.X,
                clientPoint.Y - this.AutoScrollPosition.Y);
        }

        public Rectangle ToClientRect(Rectangle rect)
        {
            Rectangle outRect = rect;
            outRect.Offset(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            return outRect;
        }

        public Rectangle ToVirtualRect(Rectangle clientRect)
        {
            Rectangle outRect = clientRect;
            outRect.Offset(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y);
            return outRect;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            // Translate the graphics context to virtual coordinates.
            pe.Graphics.TranslateTransform(
                this.AutoScrollPosition.X,
                this.AutoScrollPosition.Y);

            // Translate the clip rectangle to virtual coordinates
            // and limit it to within canvas bounds.
            var canvasClipRectangle = Rectangle.Intersect(
                new Rectangle(Point.Empty, this.CanvasSize),
                this.ToVirtualRect(pe.ClipRectangle));

            // paint the layers
            this.layers.Draw(pe.Graphics, canvasClipRectangle);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();

            base.OnMouseDown(e);
        }

        private void OnCanvasSizeChanged()
        {
            this.AutoScrollMinSize = this.CanvasSize;
            this.CanvasSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void LayersOnAreaChanged(object sender, AreaChangedEventArgs e)
        {
            var virtualRect = e.ChangedRectangle;
            var clientRect = this.ToClientRect(virtualRect);
            var intersect = Rectangle.Intersect(clientRect, this.ClientRectangle);

            if (intersect != Rectangle.Empty)
            {
                this.Invalidate(intersect);
            }
        }

        private void LayersOnFullRedraw(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
