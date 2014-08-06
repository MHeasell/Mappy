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
        private readonly IGrid<int> heights;
        private readonly int tileSize;

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

        public int SeaLevel { get; set; }

        public bool ShowSeaLevel { get; set; }

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
                    var p = new Point(x, y);
                    this.PaintHeightContours(g, p);

                    if (this.ShowSeaLevel)
                    {
                        this.PaintHeight(g, p, this.SeaLevel, Pens.Blue);
                    }
                }
            }
        }

        private void PaintHeightContours(Graphics g, Point p)
        {
            const float StartHeight = 0.8f;
            const float Interval = 4.0f;

            // find the height range
            int[] heights =
            {
                this.heights[p.X, p.Y],
                this.heights[p.X + 1, p.Y],
                this.heights[p.X, p.Y + 1],
                this.heights[p.X + 1, p.Y + 1]
            };
            float minHeight = (float)heights.Min();
            float maxHeight = (float)heights.Max();

            // offset by the base height
            minHeight -= StartHeight;
            maxHeight -= StartHeight;

            // find the range of heights that span us
            int baseVal = (int)Math.Ceiling(minHeight / Interval);
            int endVal = (int)Math.Floor(maxHeight / Interval);

            // draw them
            for (int i = baseVal; i <= endVal; i++)
            {
                this.PaintHeight(g, p, StartHeight + (i * Interval));
            }
        }

        private void PaintHeight(Graphics g, Point p, float contourHeight)
        {
            foreach (Triangle3D t in this.GenerateTriangles(p))
            {
                this.PaintTriangle(g, t, contourHeight);
            }
        }

        private void PaintHeight(Graphics g, Point p, float contourHeight, Pen pen)
        {
            foreach (Triangle3D t in this.GenerateTriangles(p))
            {
                this.PaintTriangle(g, t, contourHeight, pen);
            }
        }

        private IEnumerable<Triangle3D> GenerateTriangles(Point p)
        {
            Vector3D a = new Vector3D(p.X, p.Y, this.heights[p.X, p.Y]);
            Vector3D b = new Vector3D(p.X + 1, p.Y, this.heights[p.X + 1, p.Y]);
            Vector3D c = new Vector3D(p.X, p.Y + 1, this.heights[p.X, p.Y + 1]);
            Vector3D d = new Vector3D(p.X + 1, p.Y + 1, this.heights[p.X + 1, p.Y + 1]);

            Vector3D avg = new Vector3D(p.X + 0.5f, p.Y + 0.5f, (a.Z + b.Z + c.Z + d.Z) / 4.0f);

            yield return new Triangle3D(a, b, avg);
            yield return new Triangle3D(b, d, avg);
            yield return new Triangle3D(d, c, avg);
            yield return new Triangle3D(c, a, avg);
        }

        private void PaintTriangle(Graphics g, Triangle3D tri, float contourHeight)
        {
            int col = (int)Math.Round(Math.Pow(contourHeight / 255.0f, 1.0f / 1.5f) * 255.0f);
            using (Pen p = new Pen(Color.FromArgb(col, col, col)))
            {
                this.PaintTriangle(g, tri, contourHeight, p);
            }
        }

        private void PaintTriangle(Graphics g, Triangle3D tri, float contourHeight, Pen pen)
        {
            Plane3D clipPlane = new Plane3D(new Vector3D(0, 0, contourHeight), Vector3D.ZAxis);

            Line3D line;
            if (!Intersect.TrianglePlane(tri, clipPlane, out line))
            {
                return;
            }

            g.DrawLine(
                pen,
                (float)line.Start.X * this.tileSize,
                ((float)line.Start.Y * this.tileSize) - ((float)line.Start.Z / 2.0f),
                (float)line.End.X * this.tileSize,
                ((float)line.End.Y * this.tileSize) - ((float)line.End.Z / 2.0f));
        }
    }
}
