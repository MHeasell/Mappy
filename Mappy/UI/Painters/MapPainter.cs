namespace Mappy.UI.Painters
{
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.Collections;

    public class MapPainter : IPainter
    {
        private IGrid<Bitmap> map;
        private int tileSize;

        public MapPainter(IGrid<Bitmap> map, int tileSize)
        {
            this.map = map;
            this.tileSize = tileSize;
        }

        public void Paint(Graphics g, Rectangle clipRectangle)
        {
            IEnumerable<Point> enumer = GridUtil.EnumerateCoveringIndices(
                clipRectangle,
                new Size(this.tileSize, this.tileSize),
                new Size(this.map.Width, this.map.Height));

            foreach (Point p in enumer)
            {
                g.DrawImageUnscaled(
                    this.map[p.X, p.Y],
                    p.X * this.tileSize,
                    p.Y * this.tileSize);
            }
        }
    }
}
