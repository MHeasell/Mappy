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

        public int CellSize { get; }

        public Pen Pen { get; }

        public void Paint(Graphics g, Rectangle clipRectangle)
        {
            var startX = clipRectangle.X - Util.Mod(clipRectangle.X, this.CellSize);
            var startY = clipRectangle.Y - Util.Mod(clipRectangle.Y, this.CellSize);

            var maxX = clipRectangle.X + clipRectangle.Width;
            var maxY = clipRectangle.Y + clipRectangle.Height;

            for (var x = startX; x < maxX; x += this.CellSize)
            {
                g.DrawLine(this.Pen, x, clipRectangle.Y, x, maxY);
            }

            for (var y = startY; y < maxY; y += this.CellSize)
            {
                g.DrawLine(this.Pen, clipRectangle.X, y, maxX, y);
            }
        }
    }
}
