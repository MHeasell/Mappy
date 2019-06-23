namespace Mappy.UI.Controls
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    public sealed class MinimapControl : Control
    {
        private readonly Dictionary<int, MarkerInfo> markers = new Dictionary<int, MarkerInfo>();

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
            get => this.rectVisible;

            set
            {
                this.rectVisible = value;
                this.Invalidate(this.CoveredRect);
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color RectColor
        {
            get => this.rectColor;

            set
            {
                this.rectColor = value;
                this.Invalidate(this.CoveredRect);
            }
        }

        [DefaultValue(1.0f)]
        public float RectThickness
        {
            get => this.rectThickness;

            set
            {
                this.rectThickness = value;
                this.Invalidate(this.CoveredRect);
            }
        }

        public Rectangle ViewportRect
        {
            get => this.viewportRect;

            set
            {
                this.Invalidate(this.CoveredRect);
                this.viewportRect = value;
                this.Invalidate(this.CoveredRect);
            }
        }

        public override Image BackgroundImage
        {
            get => base.BackgroundImage;

            set
            {
                base.BackgroundImage = value;
                if (this.BackgroundImage != null)
                {
                    this.Size = base.BackgroundImage.Size;
                }
            }
        }

        protected override Size DefaultSize => new Size(252, 252);

        private Rectangle CoveredRect => new Rectangle(this.ViewportRect.Location, this.ViewportRect.Size + new Size(1, 1));

        public void SetMarker(int id, Point position, Color color)
        {
            MarkerInfo oldInfo;
            if (this.markers.TryGetValue(id, out oldInfo))
            {
                this.InvalidateMarker(oldInfo.Position);
            }

            this.markers[id] = new MarkerInfo(position, color);
            this.InvalidateMarker(position);
        }

        public void RemoveMarker(int id)
        {
            MarkerInfo oldInfo;
            if (this.markers.TryGetValue(id, out oldInfo))
            {
                this.InvalidateMarker(oldInfo.Position);
            }

            this.markers.Remove(id);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var marker in this.markers.Select(x => x.Value))
            {
                DrawMarker(e.Graphics, marker.Position, marker.Color);
            }

            if (this.RectVisible)
            {
                using (Pen p = new Pen(this.RectColor, this.RectThickness))
                {
                    e.Graphics.DrawRectangle(p, this.ViewportRect);
                }
            }
        }

        private static void DrawMarker(Graphics g, Point position, Color color)
        {
            var bounds = GetMarkerBounds(position);
            using (Brush b = new SolidBrush(color))
            {
                g.FillRectangle(b, bounds);
            }
        }

        private static Rectangle GetMarkerBounds(Point position)
        {
            return new Rectangle(
                position.X - 1,
                position.Y - 1,
                3,
                3);
        }

        private void InvalidateMarker(Point position)
        {
            this.Invalidate(GetMarkerBounds(position));
        }

        private struct MarkerInfo
        {
            public MarkerInfo(Point position, Color color)
            {
                this.Position = position;
                this.Color = color;
            }

            public Point Position { get; }

            public Color Color { get; }
        }
    }
}
