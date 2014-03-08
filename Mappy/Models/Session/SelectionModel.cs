namespace Mappy.Models.Session
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Controllers.Tags;
    using Mappy.Models.Session.BandboxBehaviours;
    using Mappy.UI.Controls;
    using Mappy.Util;

    using Point = System.Drawing.Point;

    public class SelectionModel : Notifier, ISelectionModel, ISelectionCommandHandler
    {
        private readonly IMapCommandHandler model;

        private readonly ImageLayerView view;

        private readonly ObservableCollection<GridCoordinates> selectedFeatures = new ObservableCollection<GridCoordinates>();

        private readonly IBandboxBehaviour featureBandboxBehaviour;

        private bool hasSelection;

        private int? selectedTile;

        private int? selectedStartPosition;

        private bool previousTranslationOpen;

        private int deltaX;

        private int deltaY;

        public SelectionModel(IMapCommandHandler model, ImageLayerView view)
        {
            this.model = model;
            this.view = view;

            this.selectedFeatures.CollectionChanged += this.SelectedFeaturesChanged;
            this.featureBandboxBehaviour = new FeatureBandboxBehaviour(view, this);

            this.featureBandboxBehaviour.PropertyChanged += this.BandboxBehaviourPropertyChanged;
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

        public ICollection<GridCoordinates> SelectedFeatures
        {
            get
            {
                return this.selectedFeatures;
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

        public Rectangle BandboxRectangle
        {
            get
            {
                return this.featureBandboxBehaviour.BandboxRectangle;
            }
        }

        public bool IsInSelection(int x, int y)
        {
            var item = this.view.Items.HitTest(new Point(x, y));

            if (item == null)
            {
                return false;
            }

            return this.EqualsSelectedFromTag(item.Tag);
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

            this.SelectedFeatures.Clear();
            this.MergeDownSelectedTile();
            this.SelectedStartPosition = null;
        }

        public void DeleteSelection()
        {
            if (this.SelectedFeatures.Count > 0)
            {
                foreach (var item in this.SelectedFeatures)
                {
                    this.model.RemoveFeature(item.X, item.Y);
                }

                this.SelectedFeatures.Clear();
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
            else if (this.SelectedFeatures.Count > 0)
            {
                // TODO: restore old behaviour
                // where heightmap is taken into account when placing features

                this.deltaX += x;
                this.deltaY += y;

                int quantX = this.deltaX / 16;
                int quantY = this.deltaY / 16;

                bool success = this.model.TranslateFeatureBatch(
                    this.SelectedFeatures,
                    quantX,
                    quantY);

                if (success)
                {
                    var tmp = new List<GridCoordinates>(this.SelectedFeatures);
                    this.SelectedFeatures.Clear();

                    foreach (var item in tmp)
                    {
                        this.SelectedFeatures.Add(
                            new GridCoordinates(item.X + quantX, item.Y + quantY));
                    }

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
            if (this.model.Map == null)
            {
                return;
            }

            Point? featurePos = Util.ScreenToHeightIndex(this.model.Map.Tile.HeightGrid, new Point(x, y));
            if (featurePos.HasValue)
            {
                if (this.model.TryPlaceFeature(name, featurePos.Value.X, featurePos.Value.Y))
                {
                    this.SelectFeature(Util.ToGridCoordinates(featurePos.Value));
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

        public void StartBandbox(int x, int y)
        {
            this.featureBandboxBehaviour.StartBandbox(x, y);
        }

        public void GrowBandbox(int x, int y)
        {
            this.featureBandboxBehaviour.GrowBandbox(x, y);
        }

        public void CommitBandbox()
        {
            this.featureBandboxBehaviour.CommitBandbox();
        }

        private void OnSelectionChanged()
        {
            this.HasSelection = this.SelectedFeatures.Count > 0
                    || this.SelectedTile.HasValue
                    || this.SelectedStartPosition.HasValue;
        }

        private bool EqualsSelectedFromTag(object tag)
        {
            SectionTag t = tag as SectionTag;
            if (t != null)
            {
                return t.Index == this.SelectedTile;
            }

            FeatureTag y = tag as FeatureTag;
            if (y != null)
            {
                return this.SelectedFeatures.Contains(y.Index);
            }

            StartPositionTag u = tag as StartPositionTag;
            if (u != null)
            {
                return u.Index == this.SelectedStartPosition;
            }

            return false;
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
            this.MergeDownSelectedTile();
            this.SelectedFeatures.Clear();
            this.SelectedStartPosition = index;
        }

        private void SelectTile(int index)
        {
            this.SelectedTile = index;
            this.SelectedFeatures.Clear();
            this.SelectedStartPosition = null;
        }

        private void SelectFeature(GridCoordinates coords)
        {
            this.SelectedFeatures.Clear();
            this.SelectedFeatures.Add(coords);
            this.MergeDownSelectedTile();
            this.SelectedStartPosition = null;
        }

        private void MergeDownSelectedTile()
        {
            if (this.SelectedTile.HasValue)
            {
                this.model.MergeSection(this.SelectedTile.Value);
                this.SelectedTile = null;
            }
        }

        private void SelectedFeaturesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.FireChange("SelectedFeatures");
        }

        private void BandboxBehaviourPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BandboxRectangle":
                    this.FireChange("BandboxRectangle");
                    break;
            }
        }
    }
}
