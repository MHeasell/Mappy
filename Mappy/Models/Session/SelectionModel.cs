namespace Mappy.Models.Session
{
    using System.Drawing;

    using Mappy.Controllers.Tags;
    using Mappy.UI.Controls;
    using Mappy.Util;

    public class SelectionModel : Notifier, ISelectionModel, ISelectionCommandHandler
    {
        private readonly IMapCommandHandler model;

        private readonly ImageLayerView view;

        private bool hasSelection;

        private Point? selectedFeature;

        private int? selectedTile;

        private int? selectedStartPosition;

        private bool previousTranslationOpen;

        private int deltaX;

        private int deltaY;

        public SelectionModel(IMapCommandHandler model, ImageLayerView view)
        {
            this.model = model;
            this.view = view;
        }

        public bool HasSelection
        {
            get
            {
                return this.hasSelection;
            }

            private set
            {
                this.SetField(ref this.hasSelection, value, "HasSelection");
            }
        }

        public Point? SelectedFeature
        {
            get
            {
                return this.selectedFeature;
            }

            private set
            {
                if (this.SetField(ref this.selectedFeature, value, "SelectedFeature"))
                {
                    this.OnSelectionChanged();
                }
            }
        }

        public int? SelectedTile
        {
            get
            {
                return this.selectedTile;
            }

            private set
            {
                if (this.SetField(ref this.selectedTile, value, "SelectedTile"))
                {
                    this.OnSelectionChanged();
                }
            }
        }

        public int? SelectedStartPosition
        {
            get
            {
                return this.selectedStartPosition;
            }

            private set
            {
                if (this.SetField(ref this.selectedStartPosition, value, "SelectedStartPosition"))
                {
                    this.OnSelectionChanged();
                }
            }
        }

        public bool SelectAtPoint(int x, int y)
        {
            if (this.previousTranslationOpen)
            {
                this.FlushTranslation();
            }

            var item = this.view.Items.HitTest(new Point(x, y));
            if (item == null)
            {
                this.ClearSelection();
                return false;
            }

            this.SelectFromTag(item.Tag);
            return true;
        }

        public void ClearSelection()
        {
            if (this.previousTranslationOpen)
            {
                this.FlushTranslation();
            }

            this.SelectedFeature = null;
            this.SelectedTile = null;
            this.SelectedStartPosition = null;
        }

        public void DeleteSelection()
        {
            if (this.SelectedFeature.HasValue)
            {
                this.model.RemoveFeature(this.SelectedFeature.Value);
                this.SelectedFeature = null;
            }
            else if (this.SelectedTile.HasValue)
            {
                this.model.RemoveSection(this.SelectedTile.Value);
                this.SelectedTile = null;
            }
            else if (this.SelectedStartPosition.HasValue)
            {
                this.model.RemoveStartPosition(this.SelectedStartPosition.Value);
                this.SelectedStartPosition = null;
            }
        }

        public void TranslateSelection(int x, int y)
        {
            if (this.SelectedStartPosition.HasValue)
            {
                this.model.TranslateStartPosition(
                    this.SelectedStartPosition.Value,
                    x,
                    y);
            }
            else if (this.SelectedTile.HasValue)
            {
                this.deltaX += x;
                this.deltaY += y;

                this.model.TranslateSection(
                    this.SelectedTile.Value,
                    this.deltaX / 32,
                    this.deltaY / 32);

                this.deltaX %= 32;
                this.deltaY %= 32;
            }
            else if (this.SelectedFeature.HasValue)
            {
                // TODO: restore old behaviour
                // where heightmap is taken into account when placing features

                this.deltaX += x;
                this.deltaY += y;

                int quantX = this.deltaX / 16;
                int quantY = this.deltaY / 16;

                bool success = this.model.TranslateFeature(
                    this.SelectedFeature.Value,
                    quantX,
                    quantY);

                if (success)
                {
                    this.SelectedFeature = new Point(
                        this.SelectedFeature.Value.X + quantX,
                        this.SelectedFeature.Value.Y + quantY);

                    this.deltaX %= 16;
                    this.deltaY %= 16;
                }
            }

            this.previousTranslationOpen = true;
        }

        public void FlushTranslation()
        {
            this.deltaX = 0;
            this.deltaY = 0;
            this.previousTranslationOpen = false;
            this.model.FlushTranslation();
        }

        public void DragDropFeature(string name, int x, int y)
        {
            Point? featurePos = Util.ScreenToHeightIndex(this.model.Map.Tile.HeightGrid, new Point(x, y));
            if (featurePos.HasValue)
            {
                if (this.model.TryPlaceFeature(name, featurePos.Value.X, featurePos.Value.Y))
                {
                    this.SelectFeature(featurePos.Value);
                }
            }
        }

        public void DragDropTile(int id, int x, int y)
        {
            int quantX = x / 32;
            int quantY = y / 32;
            int index = this.model.PlaceSection(id, quantX, quantY);

            if (index != -1)
            {
                this.SelectTile(index);
            }
        }

        public void DragDropStartPosition(int index, int x, int y)
        {
            this.model.SetStartPosition(index, x, y);

            this.SelectStartPosition(index);
        }

        private void OnSelectionChanged()
        {
            this.HasSelection = this.SelectedFeature.HasValue
                    || this.SelectedTile.HasValue
                    || this.SelectedStartPosition.HasValue;
        }

        private void SelectFromTag(object tag)
        {
            SectionTag t = tag as SectionTag;
            if (t != null)
            {
                this.SelectTile(t.Index);
                return;
            }

            FeatureTag y = tag as FeatureTag;
            if (y != null)
            {
                this.SelectFeature(y.Index);
                return;
            }

            StartPositionTag u = tag as StartPositionTag;
            if (u != null)
            {
                this.SelectStartPosition(u.Index);
                return;
            }
        }

        private void SelectStartPosition(int index)
        {
            this.SelectedTile = null;
            this.SelectedFeature = null;
            this.SelectedStartPosition = index;
        }

        private void SelectTile(int index)
        {
            this.SelectedTile = index;
            this.SelectedFeature = null;
            this.SelectedStartPosition = null;
        }

        private void SelectFeature(Point coords)
        {
            this.SelectedFeature = coords;
            this.SelectedTile = null;
            this.SelectedStartPosition = null;
        }
    }
}
