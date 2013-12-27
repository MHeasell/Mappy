namespace Mappy.UI.Painters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Geometry;

    using Mappy.Collections;

    public class ContourHeightPainter : IPainter
    {
        private IGrid<int> heights;
        private int tileSize;

        public ContourHeightPainter(IGrid<int> heights, int tileSize, int subDivisions)
        {
            this.heights = heights;
            this.tileSize = tileSize;
            this.SubDivisions = subDivisions;
        }

        public ContourHeightPainter(IGrid<int> heights, int tileSize)
            : this(heights, tileSize, 64)
        {
        }

        public int SubDivisions { get; set; }

        public void Paint(Graphics g, Rectangle clipRectangle)
        {
            int startX = clipRectangle.X / this.tileSize;
            int startY = clipRectangle.Y / this.tileSize;
            
            // start a tile before to account for bleed in
            startX -= 1;
            startY -= 1;
            
            // cap start to lower bounds of grid
            startX = Math.Max(0, startX);
            startY = Math.Max(0, startY);

            int widthTiles = clipRectangle.Width / this.tileSize;
            int heightTiles = clipRectangle.Height / this.tileSize;

            // overdraw to account for integer division
            widthTiles += 3;
            heightTiles += 3;

            // extend y axis draw as tiles from up to 128px below can affect us
            heightTiles += 8;

            // cap end to upper bounds
            widthTiles = Math.Min(this.heights.Width - startX - 1, widthTiles);
            heightTiles = Math.Min(this.heights.Height - startY - 1, heightTiles);

            this.PaintTileRange(
                g,
                new Rectangle(startX, startY, widthTiles, heightTiles));
        }

        private void PaintTileRange(Graphics g, Rectangle tileRange)
        {
            for (int y = tileRange.Y; y < tileRange.Y + tileRange.Height; y++)
            {
                for (int x = tileRange.X; x < tileRange.X + tileRange.Width; x++)
                {
                    this.PaintHeightContours(g, new Point(x, y));
                }
            }
        }

        private void PaintHeightContours(Graphics g, Point p)
        {
            float startHeight = 0.8f;
            float interval = 4.0f;

            // find the height range
            int[] heights =
            {
                this.heights.Get(p.X, p.Y),
                this.heights.Get(p.X + 1, p.Y),
                this.heights.Get(p.X, p.Y + 1),
                this.heights.Get(p.X + 1, p.Y + 1)
            };
            float minHeight = (float)heights.Min();
            float maxHeight = (float)heights.Max();

            // offset by the base height
            minHeight -= startHeight;
            maxHeight -= startHeight;

            // find the range of heights that span us
            int baseVal = (int)Math.Ceiling(minHeight / interval);
            int endVal = (int)Math.Floor(maxHeight / interval);

            // draw them
            for (int i = baseVal; i <= endVal; i++)
            {
                this.PaintHeight(g, p, startHeight + (i * interval));
            }
        }

        private void PaintHeight(Graphics g, Point p, float contourHeight)
        {
            foreach (Triangle3D t in this.GenerateTriangles(p))
            {
                this.PaintTriangle(g, t, contourHeight);
            }
        }

        private IEnumerable<Triangle3D> GenerateTriangles(Point p)
        {
            Vector3D a = new Vector3D(p.X, p.Y, this.heights.Get(p.X, p.Y));
            Vector3D b = new Vector3D(p.X + 1, p.Y, this.heights.Get(p.X + 1, p.Y));
            Vector3D c = new Vector3D(p.X, p.Y + 1, this.heights.Get(p.X, p.Y + 1));
            Vector3D d = new Vector3D(p.X + 1, p.Y + 1, this.heights.Get(p.X + 1, p.Y + 1));

            Vector3D avg = new Vector3D(p.X + 0.5f, p.Y + 0.5f, (a.Z + b.Z + c.Z + d.Z) / 4.0f);

            yield return new Triangle3D(a, b, avg);
            yield return new Triangle3D(b, d, avg);
            yield return new Triangle3D(d, c, avg);
            yield return new Triangle3D(c, a, avg);
        }

        private void PaintTriangle(Graphics g, Triangle3D tri, float contourHeight)
        {
            Plane3D clipPlane = new Plane3D(new Vector3D(0, 0, contourHeight), Vector3D.ZAxis);

            Line3D line;
            if (!Intersect.TrianglePlane(tri, clipPlane, out line))
            {
                return;
            }

            int col = (int)Math.Round(Math.Pow(contourHeight / 255.0f, 1.0f / 1.5f) * 255.0f);
            using (Pen p = new Pen(Color.FromArgb(col, col, col)))
            {
                g.DrawLine(
                    p,
                    (float)line.Start.X * this.tileSize,
                    ((float)line.Start.Y * this.tileSize) - ((float)line.Start.Z / 2.0f),
                    (float)line.End.X * this.tileSize,
                    ((float)line.End.Y * this.tileSize) - ((float)line.End.Z / 2.0f));
            }
        }
    }
}
