namespace Mappy.Minimap
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class MinimapControl : Control
    {
        private bool rectVisible = true;
        private Color rectColor = Color.Black;
        private float rectThickness = 1.0f;
        private Rectangle viewportRect;

        public MinimapControl()
        {
            this.DoubleBuffered = true;
        }

        [DefaultValue(true)]
        public bool RectVisible
        {
            get
            {
                return this.rectVisible;
            }

            set
            {
                this.rectVisible = value;
                this.Invalidate(this.CoveredRect);
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color RectColor
        {
            get
            {
                return this.rectColor;
            }

            set
            {
                this.rectColor = value;
                this.Invalidate(this.CoveredRect);
            }
        }

        [DefaultValue(1.0f)]
        public float RectThickness
        {
            get
            {
                return this.rectThickness;
            }

            set
            {
                this.rectThickness = value;
                this.Invalidate(this.CoveredRect);
            }
        }

        public Rectangle ViewportRect
        {
            get
            {
                return this.viewportRect;
            }

            set
            {
                this.Invalidate(this.CoveredRect);
                this.viewportRect = value;
                this.Invalidate(this.CoveredRect);
            }
        }

        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }

            set
            {
                base.BackgroundImage = value;
                this.Size = base.BackgroundImage.Size;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(252, 252);
            }
        }

        private Rectangle CoveredRect
        {
            get
            {
                return new Rectangle(this.ViewportRect.Location, this.ViewportRect.Size + new Size(1, 1));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.RectVisible)
            {
                using (Pen p = new Pen(this.RectColor, this.RectThickness))
                {
                    e.Graphics.DrawRectangle(p, this.ViewportRect);
                }
            }
        }
    }
}
