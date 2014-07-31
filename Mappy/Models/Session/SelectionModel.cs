namespace Mappy.Models.Session
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.Models.Session.BandboxBehaviours;
    using Mappy.Presentation;
    using Mappy.Util;

    using Point = System.Drawing.Point;

    public class SelectionModel : Notifier, IMysteryModel
    {
        private readonly IMainModel model;

        private readonly ObservableCollection<GridCoordinates> selectedFeatures = new ObservableCollection<GridCoordinates>();

        private IBandboxBehaviour bandboxBehaviour;

        private bool hasSelection;

        private int? selectedTile;

        private int? selectedStartPosition;

        private bool previousTranslationOpen;

        private int deltaX;

        private int deltaY;

        public SelectionModel(IMainModel model)
        {
            this.model = model;

            this.selectedFeatures.CollectionChanged += this.SelectedFeaturesChanged;
            ////this.bandboxBehaviour = new FeatureBandboxBehaviour(view, this);
            this.bandboxBehaviour = new TileBandboxBehaviour(this.model, this);

            this.bandboxBehaviour.PropertyChanged += this.BandboxBehaviourPropertyChanged;

            this.model.PropertyChanged += this.ModelOnPropertyChanged;
        }

        public event EventHandler<SparseGridEventArgs> FeaturesChanged
        {
            add
            {
                this.model.FeaturesChanged += value;
            }

            remove
            {
                this.model.FeaturesChanged -= value;
            }
        }

        public event EventHandler<ListChangedEventArgs> TilesChanged
        {
            add
            {
                this.model.TilesChanged += value;
            }

            remove
            {
                this.model.TilesChanged -= value;
            }
        }

        public event EventHandler<GridEventArgs> BaseTileGraphicsChanged
        {
            add
            {
                this.model.BaseTileGraphicsChanged += value;
            }

            remove
            {
                this.model.BaseTileGraphicsChanged -= value;
            }
        }

        public event EventHandler<GridEventArgs> BaseTileHeightChanged
        {
            add
            {
                this.model.BaseTileHeightChanged += value;
            }

            remove
            {
                this.model.BaseTileHeightChanged -= value;
            }
        }

        public event EventHandler<StartPositionChangedEventArgs> StartPositionChanged
        {
            add
            {
                this.model.StartPositionChanged += value;
            }

            remove
            {
                this.model.StartPositionChanged -= value;
            }
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

        public ISparseGrid<Feature> Features
        {
            get
            {
                return this.model.Features;
            }
        }

        public IList<Positioned<IMapTile>> FloatingTiles
        {
            get
            {
                return this.model.FloatingTiles;
            }
        }

        public IMapTile BaseTile
        {
            get
            {
                return this.model.BaseTile;
            }
        }

        public int MapWidth
        {
            get
            {
                return this.model.MapWidth;
            }
        }

        public int MapHeight
        {
            get
            {
                return this.model.MapHeight;
            }
        }

        public bool MapOpen
        {
            get
            {
                return this.model.MapOpen;
            }
        }

        public bool GridVisible
        {
            get
            {
                return this.model.GridVisible;
            }
        }

        public Color GridColor
        {
            get
            {
                return this.model.GridColor;
            }
        }

        public Size GridSize
        {
            get
            {
                return this.model.GridSize;
            }
        }

        public bool HeightmapVisible
        {
            get
            {
                return this.model.HeightmapVisible;
            }
        }

        public bool FeaturesVisible
        {
            get
            {
                return this.model.FeaturesVisible;
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
                return this.bandboxBehaviour.BandboxRectangle;
            }
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

        public Point? GetStartPosition(int index)
        {
            return this.model.GetStartPosition(index);
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
            if (!this.model.MapOpen)
            {
                return;
            }

            Point? featurePos = this.model.ScreenToHeightIndex(x, y);
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
            this.bandboxBehaviour.StartBandbox(x, y);
        }

        public void GrowBandbox(int x, int y)
        {
            this.bandboxBehaviour.GrowBandbox(x, y);
        }

        public void CommitBandbox()
        {
            this.bandboxBehaviour.CommitBandbox();
        }

        public void SelectTile(int index)
        {
            this.SelectedTile = index;
            this.SelectedFeatures.Clear();
            this.SelectedStartPosition = null;
        }

        public void SelectStartPosition(int index)
        {
            this.MergeDownSelectedTile();
            this.SelectedFeatures.Clear();
            this.SelectedStartPosition = index;
        }

        public void SelectFeature(GridCoordinates coords)
        {
            this.SelectedFeatures.Clear();
            this.SelectedFeatures.Add(coords);
            this.MergeDownSelectedTile();
            this.SelectedStartPosition = null;
        }

        private void OnSelectionChanged()
        {
            this.HasSelection = this.SelectedFeatures.Count > 0
                    || this.SelectedTile.HasValue
                    || this.SelectedStartPosition.HasValue;
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

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var passthroughProperties = new HashSet<string>
                {
                    "GridVisible",
                    "GridColor",
                    "GridSize",
                    "HeightmapVisible",
                    "FeaturesVisible",
                    "Features",
                    "FloatingTiles",
                    "BaseTile",
                    "MapWidth",
                    "MapHeight",
                    "MapOpen",
                };

            if (passthroughProperties.Contains(propertyChangedEventArgs.PropertyName))
            {
                this.FireChange(propertyChangedEventArgs.PropertyName);
            }
        }
    }
}
