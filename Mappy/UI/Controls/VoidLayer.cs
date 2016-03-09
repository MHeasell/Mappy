namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;
    using System.Reactive.Linq;

    using Mappy.Collections;
    using Mappy.Models;
    using Mappy.UI.Painters;

    public sealed class VoidLayer : AbstractLayer, IDisposable
    {
        private const int CellSize = 16;

        private readonly IPainter painter;

        private readonly IDisposable eventSubscription;

        public VoidLayer(UndoableMapModel map)
        {
            var grid = new BindingGrid<bool>(map.Voids);
            this.painter = new VoidPainter(map.Voids, map.BaseTile.HeightGrid, new Size(CellSize, CellSize));

            var voidChangeEvents = Observable.FromEventPattern<GridEventArgs>(
                e => grid.CellsChanged += e,
                e => grid.CellsChanged -= e)
                .Select(e => new GridCoordinates(e.EventArgs.X, e.EventArgs.Y));

            var heightchangeEvents = Observable.FromEventPattern<GridEventArgs>(
                e => map.BaseTileHeightChanged += e,
                e => map.BaseTileHeightChanged -= e)
                .Select(e => new GridCoordinates(e.EventArgs.X, e.EventArgs.Y));

            this.eventSubscription = voidChangeEvents.Merge(heightchangeEvents)
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
            // Due to the projection, one cell could influence the rendering
            // of a strip containing up to 8 cells in screen space,
            // covering the heights from 0 to 255.
            return new Rectangle(
                cell.X * CellSize,
                (cell.Y - 8) * CellSize,
                CellSize,
                CellSize * 8);
        }
    }
}
