namespace Mappy.Controllers
{
    using System.Drawing;

    using Mappy.Data;
    using Mappy.Models;
    using Mappy.UI.Controls;
    using Mappy.Util;

    public class MapCommandHandler
    {
        private readonly ImageLayerView view;

        private readonly IMapPresenterModel model;

        private bool mouseDown;

        private Point lastMousePos;

        private Point delta;

        public MapCommandHandler(ImageLayerView view, IMapPresenterModel model)
        {
            this.view = view;
            this.model = model;
        }

        public void MouseDown(int virtualX, int virtualY)
        {
            this.mouseDown = true;
            this.lastMousePos = new Point(virtualX, virtualY);
            this.delta = new Point();
        }

        public void MouseMove(int virtualX, int virtualY)
        {
            try
            {
                if (!this.mouseDown)
                {
                    return;
                }

                if (this.view.SelectedItem == null)
                {
                    return;
                }

                MapPresenter.ItemTag tag = (MapPresenter.ItemTag)this.view.SelectedItem.Tag;
                tag.DragTo(new Point(virtualX, virtualY));
            }
            finally
            {
                this.lastMousePos = new Point(virtualX, virtualY);
            }
        }

        public void MouseUp(int virtualX, int virtualY)
        {
            this.model.FlushTranslation();
            this.mouseDown = false;
        }

        public void DragSectionTo(Positioned<IMapTile> t, Point location)
        {
            Point delta = new Point(
                location.X - this.lastMousePos.X,
                location.Y - this.lastMousePos.Y);

            this.delta.Offset(delta);

            this.model.TranslateSection(
                t,
                this.delta.X / 32,
                this.delta.Y / 32);

            this.delta.X %= 32;
            this.delta.Y %= 32;
        }

        public void DragFeatureTo(Point featureCoords, Point location)
        {
            Point? pos = Util.ScreenToHeightIndex(
                    this.model.Map.Tile.HeightGrid,
                    location);
            if (!pos.HasValue)
            {
                return;
            }

            this.model.TranslateFeature(
                featureCoords,
                pos.Value.X - featureCoords.X,
                pos.Value.Y - featureCoords.Y);
        }
    }
}
