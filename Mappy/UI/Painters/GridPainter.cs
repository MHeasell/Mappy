namespace Mappy.UI.Painters
{
    using System.Drawing;

    using Mappy.Util;

    public class GridPainter : IPainter
    {
        public GridPainter(int cellSize, Pen pen)
        {
            this.CellSize = cellSize;
            this.Pen = pen;
        }

        public int CellSize { get; private set; }

        public Pen Pen { get; private set; }

        public void Paint(Graphics g, Rectangle clipRectangle)
        {
            int startX = clipRectangle.X - Util.Mod(clipRectangle.X, this.CellSize);
            int startY = clipRectangle.Y - Util.Mod(clipRectangle.Y, this.CellSize);

            int maxX = clipRectangle.X + clipRectangle.Width;
            int maxY = clipRectangle.Y + clipRectangle.Height;

            for (int x = startX; x < maxX; x += this.CellSize)
            {
                g.DrawLine(this.Pen, x, clipRectangle.Y, x, maxY);
            }

            for (int y = startY; y < maxY; y += this.CellSize)
            {
                g.DrawLine(this.Pen, clipRectangle.X, y, maxX, y);
            }
        }
    }
}
