namespace Mappy.UI.Painters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Mappy.Collections;

    public sealed class VoidPainter : IPainter, IDisposable
    {
        private readonly IGrid<bool> grid;

        private readonly IGrid<int> heights;

        private readonly Size tileSize;

        private readonly Brush brush = new SolidBrush(Color.FromArgb(127, 255, 0, 0));

        public VoidPainter(IGrid<bool> grid, IGrid<int> heights, Size tileSize)
        {
            this.grid = grid;
            this.heights = heights;
            this.tileSize = tileSize;
        }

        public void Paint(Graphics g, Rectangle clipRectangle)
        {
            var polygons = this.EnumerateCoveredCells(clipRectangle)
                .Where(p => this.grid.Get(p.X, p.Y))
                .Select(this.GetHeightPolygon);

            foreach (var polygon in polygons)
            {
                g.FillPolygon(this.brush, polygon);
            }
        }

        public void Dispose()
        {
            this.brush.Dispose();
        }

        private Point[] GetHeightPolygon(GridCoordinates cell)
        {
            return new[]
            {
                this.GetProjectedPoint(cell.X, cell.Y),
                this.GetProjectedPoint(cell.X + 1, cell.Y),
                this.GetProjectedPoint(cell.X + 1, cell.Y + 1),
                this.GetProjectedPoint(cell.X, cell.Y + 1)
            };
        }

        private IEnumerable<GridCoordinates> EnumerateCoveredCells(Rectangle clipRectangle)
        {
            var startCell = new GridCoordinates(
                clipRectangle.X / this.tileSize.Width,
                clipRectangle.Y / this.tileSize.Height);

            if (startCell.X < 0
                || startCell.Y < 0
                || startCell.X >= this.grid.Width - 1
                || startCell.Y >= this.grid.Width - 1)
            {
                yield break;
            }

            var cellsWidth = clipRectangle.Width / this.tileSize.Width;
            var cellsHeight = clipRectangle.Height / this.tileSize.Height;

            // Overdraw on the bottom and right
            // so that we compensate for integer division.
            // +1 for the remainder of division from computing startCell
            // +1 for the remainder of division from computing cellsWidth and cellsHeight
            cellsWidth += 2;
            cellsHeight += 2;

            // Overdraw height by 8 tiles (128 pixels)
            // to account for the projection angle.
            cellsHeight += 8;

            // prevent escaping the grid
            var stopCell = new GridCoordinates(
                Math.Min(startCell.X + cellsWidth, this.grid.Width - 1),
                Math.Min(startCell.Y + cellsHeight, this.grid.Height - 1));

            for (var y = startCell.Y; y < stopCell.Y; y++)
            {
                for (var x = startCell.X; x < stopCell.X; x++)
                {
                    yield return new GridCoordinates(x, y);
                }
            }
        }

        private Point GetProjectedPoint(int x, int y)
        {
            var extraY = this.heights.Get(x, y) / 2;
            return new Point(x * this.tileSize.Width, (y * this.tileSize.Height) - extraY);
        }
    }
}
