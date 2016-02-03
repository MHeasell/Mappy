namespace Mappy.UI.Painters
{
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.Collections;

    public class BitmapGridPainter : IPainter
    {
        private readonly IGrid<Bitmap> map;
        private readonly int tileSize;

        public BitmapGridPainter(IGrid<Bitmap> map, int tileSize)
        {
            this.map = map;
            this.tileSize = tileSize;
            this.BackgroundColor = Color.Transparent;
        }

        public Color BackgroundColor { get; set; }

        public void Paint(Graphics g, Rectangle clipRectangle)
        {
            IEnumerable<Point> enumer = GridUtil.EnumerateCoveringIndices(
                clipRectangle,
                new Size(this.tileSize, this.tileSize),
                new Size(this.map.Width, this.map.Height));

            using (Brush backgroundBrush = new SolidBrush(this.BackgroundColor))
            {
                foreach (Point p in enumer)
                {
                    var img = this.map.Get(p.X, p.Y);
                    if (img != null)
                    {
                        g.DrawImageUnscaled(
                            this.map.Get(p.X, p.Y),
                            p.X * this.tileSize,
                            p.Y * this.tileSize);
                    }
                    else
                    {
                        g.FillRectangle(
                            backgroundBrush,
                            p.X * this.tileSize,
                            p.Y * this.tileSize,
                            this.tileSize,
                            this.tileSize);
                    }
                }
            }
        }
    }
}
