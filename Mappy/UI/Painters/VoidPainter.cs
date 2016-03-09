namespace Mappy.UI.Painters
{
    using System;
    using System.Drawing;
    using System.Linq;

    using Mappy.Collections;

    public sealed class VoidPainter : IPainter, IDisposable
    {
        private readonly IGrid<bool> grid;

        private readonly Size tileSize;

        private readonly Brush brush = new SolidBrush(Color.FromArgb(127, 255, 0, 0));

        public VoidPainter(IGrid<bool> grid, Size tileSize)
        {
            this.grid = grid;
            this.tileSize = tileSize;
        }

        public void Paint(Graphics g, Rectangle clipRectangle)
        {
            var gridSize = new Size(this.grid.Width, this.grid.Height);
            var enumer = GridUtil.EnumerateCoveringIndices(
                clipRectangle,
                this.tileSize,
                gridSize);

            foreach (var cell in enumer.Where(p => this.grid.Get(p.X, p.Y)))
            {
                g.FillRectangle(
                    this.brush,
                    cell.X * this.tileSize.Width,
                    cell.Y * this.tileSize.Height,
                    this.tileSize.Width,
                    this.tileSize.Height);
            }
        }

        public void Dispose()
        {
            this.brush.Dispose();
        }
    }
}
