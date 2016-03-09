namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;
    using System.Reactive.Linq;

    using Mappy.Collections;
    using Mappy.UI.Painters;

    public sealed class VoidLayer : AbstractLayer, IDisposable
    {
        private const int CellSize = 16;

        private readonly IPainter painter;

        private readonly IDisposable eventSubscription;

        public VoidLayer(BindingGrid<bool> grid)
        {
            this.painter = new VoidPainter(grid, new Size(CellSize, CellSize));

            this.eventSubscription = Observable.FromEventPattern<GridEventArgs>(
                e => grid.CellsChanged += e,
                e => grid.CellsChanged -= e)
                .Select(e => new GridCoordinates(e.EventArgs.X, e.EventArgs.Y))
                .Select(GetCellRectangle)
                .Subscribe(this.OnLayerChanged);
        }

        public void Dispose()
        {
            this.eventSubscription.Dispose();
        }

        protected override void DoDraw(Graphics graphics, Rectangle clipRectangle)
        {
            this.painter.Paint(graphics, clipRectangle);
        }

        private static Rectangle GetCellRectangle(GridCoordinates cell)
        {
            return new Rectangle(cell.X * CellSize, cell.Y * CellSize, CellSize, CellSize);
        }
    }
}
