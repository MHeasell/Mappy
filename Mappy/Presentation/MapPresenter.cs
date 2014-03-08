namespace Mappy.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Controllers.Tags;
    using Mappy.Data;
    using Mappy.Models;
    using Mappy.Models.Session;
    using Mappy.UI.Controls;
    using Mappy.UI.Drawables;
    using Mappy.Util;

    public class MapPresenter
    {
        private const int BandboxDepth = 100000000;

        private static readonly Color BandboxFillColor = Color.FromArgb(127, Color.Blue);
        private static readonly Color BandboxBorderColor = Color.FromArgb(127, Color.Black);

        private static readonly IDrawable[] StartPositionImages = new IDrawable[10];

        private readonly ImageLayerView view;
        private readonly IMapDataModel model;

        private readonly List<ImageLayerCollection.Item> tileMapping = new List<ImageLayerCollection.Item>();
        private readonly IDictionary<GridCoordinates, ImageLayerCollection.Item> featureMapping = new Dictionary<GridCoordinates, ImageLayerCollection.Item>();

        private readonly ImageLayerCollection.Item[] startPositionMapping = new ImageLayerCollection.Item[10];

        private readonly ISelectionModel selectionModel;

        private readonly IViewOptionsModel viewOptionsModel;

        private DrawableTile baseTile;

        private ImageLayerCollection.Item bandboxMapping;

        static MapPresenter()
        {
            for (int i = 0; i < 10; i++)
            {
                var image = new DrawableBitmap(Util.GetStartImage(i + 1));
                MapPresenter.StartPositionImages[i] = image;
            }
        }

        public MapPresenter(ImageLayerView view, IMapDataModel model, ISelectionModel selectionModel, IViewOptionsModel viewOptionsModel)
        {
            this.view = view;
            this.model = model;
            this.selectionModel = selectionModel;
            this.viewOptionsModel = viewOptionsModel;

            this.model.PropertyChanged += this.ModelPropertyChanged;
            this.selectionModel.PropertyChanged += this.SelectionModelPropertyChanged;
            this.viewOptionsModel.PropertyChanged += this.ViewOptionsModelPropertyChanged;

            this.PopulateView();
            this.WireMap();

            this.view.GridVisible = this.viewOptionsModel.GridVisible;
            this.view.GridColor = this.viewOptionsModel.GridColor;
            this.view.GridSize = this.viewOptionsModel.GridSize;
        }

        private void WireMap()
        {
            if (this.model.Map == null)
            {
                return;
            }

            this.model.Map.Features.EntriesChanged += this.FeatureChanged;
            this.model.Map.FloatingTiles.ListChanged += this.TilesChanged;

            this.model.Map.Tile.TileGridChanged += this.BaseTileChanged;
            this.model.Map.Tile.HeightGridChanged += this.BaseTileChanged;

            foreach (var t in this.model.Map.FloatingTiles)
            {
                t.LocationChanged += this.TileLocationChanged;
            }

            this.model.Map.Attributes.StartPositionChanged += this.StartPositionChanged;
        }

        private void PopulateView()
        {
            this.tileMapping.Clear();
            this.featureMapping.Clear();
            this.view.Items.Clear();
            this.baseTile = null;

            if (this.model.Map == null)
            {
                this.view.CanvasSize = Size.Empty;
                return;
            }

            this.view.CanvasSize = new Size(
                this.model.Map.Tile.TileGrid.Width * 32,
                this.model.Map.Tile.TileGrid.Height * 32);

            this.baseTile = new DrawableTile(this.model.Map.Tile);
            this.baseTile.DrawHeightMap = this.viewOptionsModel.HeightmapVisible;
            ImageLayerCollection.Item baseItem = new ImageLayerCollection.Item(
                0,
                0,
                -1,
                this.baseTile);

            baseItem.Locked = true;

            this.view.Items.Add(baseItem);

            int count = 0;
            foreach (Positioned<IMapTile> t in this.model.Map.FloatingTiles)
            {
                this.InsertTile(t, count++);
            }

            foreach (var f in this.model.Map.Features.CoordinateEntries)
            {
                this.InsertFeature(f.Value, f.Key.X, f.Key.Y);
            }

            for (int i = 0; i < 10; i++)
            {
                this.UpdateStartPosition(i);
            }
        }

        private void InsertTile(Positioned<IMapTile> t, int index)
        {
            ImageLayerCollection.Item i = new ImageLayerCollection.Item(
                    t.Location.X * 32,
                    t.Location.Y * 32,
                    index,
                    new DrawableTile(t.Item));
            i.Tag = new SectionTag(index);
            this.tileMapping.Insert(index, i);
            this.view.Items.Add(i);
        }

        private void RemoveTile(int index)
        {
            ImageLayerCollection.Item item = this.tileMapping[index];
            this.view.Items.Remove(item);
            this.view.RemoveFromSelection(item);
            this.tileMapping.RemoveAt(index);
        }

        private void InsertFeature(Feature f, int x, int y)
        {
            var coords = new GridCoordinates(x, y);
            int index = this.ToFeatureIndex(x, y);
            Rectangle r = f.GetDrawBounds(this.model.Map.Tile.HeightGrid, x, y);
            ImageLayerCollection.Item i = new ImageLayerCollection.Item(
                    r.X,
                    r.Y,
                    index + 1000, // magic number to separate from tiles
                    new DrawableBitmap(f.Image));
            i.Tag = new FeatureTag(coords);
            i.Visible = this.viewOptionsModel.FeaturesVisible;
            this.featureMapping[coords] = i;
            this.view.Items.Add(i);
        }

        private bool RemoveFeature(GridCoordinates coords)
        {
            if (this.featureMapping.ContainsKey(coords))
            {
                ImageLayerCollection.Item item = this.featureMapping[coords];
                this.view.Items.Remove(item);
                this.view.RemoveFromSelection(item);
                this.featureMapping.Remove(coords);
                return true;
            }

            return false;
        }

        private void MoveFeature(GridCoordinates oldIndex, GridCoordinates newIndex)
        {
            var old = this.featureMapping[oldIndex];

            bool isSelected = this.view.SelectedItemsContains(old);

            this.RemoveFeature(oldIndex);
            this.InsertFeature(newIndex);

            if (isSelected)
            {
                this.view.AddToSelection(this.featureMapping[newIndex]);
            }
        }

        private void UpdateFeature(GridCoordinates index)
        {
            this.RemoveFeature(index);

            this.InsertFeature(index);
        }

        private void InsertFeature(GridCoordinates p)
        {
            Feature f;
            if (this.model.Map.Features.TryGetValue(p.X, p.Y, out f))
            {
                this.InsertFeature(f, p.X, p.Y);
            }
        }

        private GridCoordinates ToFeaturePoint(int index)
        {
            int x = index % this.model.Map.Features.Width;
            int y = index / this.model.Map.Features.Width;
            var p = new GridCoordinates(x, y);
            return p;
        }

        private int ToFeatureIndex(GridCoordinates p)
        {
            return this.ToFeatureIndex(p.X, p.Y);
        }

        private int ToFeatureIndex(int x, int y)
        {
            return (y * this.model.Map.Features.Width) + x;
        }

        private void RefreshFeatureVisibility()
        {
            foreach (var i in this.featureMapping.Values)
            {
                i.Visible = this.viewOptionsModel.FeaturesVisible;
            }
        }

        private void RefreshHeightmapVisibility()
        {
            if (this.baseTile == null)
            {
                return;
            }

            this.baseTile.DrawHeightMap = this.viewOptionsModel.HeightmapVisible;
            this.view.Invalidate();
        }

        private void SelectionModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedTile":
                case "SelectedFeatures":
                case "SelectedStartPosition":
                    this.RefreshSelection();
                    break;
                case "BandboxRectangle":
                    this.UpdateBandbox();
                    break;
            }
        }

        private void RefreshSelection()
        {
            this.view.ClearSelection();

            if (this.selectionModel.SelectedTile.HasValue)
            {
                this.view.AddToSelection(this.tileMapping[this.selectionModel.SelectedTile.Value]);
            }
            else if (this.selectionModel.SelectedFeatures.Count > 0)
            {
                foreach (var item in this.selectionModel.SelectedFeatures)
                {
                    this.view.AddToSelection(this.featureMapping[item]);
                }
            }
            else if (this.selectionModel.SelectedStartPosition.HasValue)
            {
                this.view.AddToSelection(this.startPositionMapping[this.selectionModel.SelectedStartPosition.Value]);
            }
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Map":
                    this.WireMap();
                    this.PopulateView();
                    this.RefreshHeightmapVisibility();
                    break;
            }
        }

        private void ViewOptionsModelPropertyChanged(object sneder, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FeaturesVisible":
                    this.RefreshFeatureVisibility();
                    break;
                case "HeightmapVisible":
                    this.RefreshHeightmapVisibility();
                    break;
                case "GridVisible":
                    this.view.GridVisible = this.viewOptionsModel.GridVisible;
                    break;
                case "GridColor":
                    this.view.GridColor = this.viewOptionsModel.GridColor;
                    break;
                case "GridSize":
                    this.view.GridSize = this.viewOptionsModel.GridSize;
                    break;
            }
        }

        private void StartPositionChanged(object sender, StartPositionChangedEventArgs e)
        {
            this.UpdateStartPosition(e.Index);
        }

        private void UpdateStartPosition(int index)
        {
            bool selected = false;

            if (this.startPositionMapping[index] != null)
            {
                var mapping = this.startPositionMapping[index];
                selected = this.view.SelectedItemsContains(mapping);
                this.view.Items.Remove(mapping);
                this.view.RemoveFromSelection(mapping);
                this.startPositionMapping[index] = null;
            }

            Point? p = this.model.Map.Attributes.GetStartPosition(index);
            if (p.HasValue)
            {
                IDrawable img = StartPositionImages[index];
                var i = new ImageLayerCollection.Item(
                    p.Value.X - (img.Width / 2),
                    p.Value.Y - 58,
                    int.MaxValue,
                    img);
                i.Tag = new StartPositionTag(index);
                this.startPositionMapping[index] = i;
                this.view.Items.Add(i);

                if (selected)
                {
                    this.view.AddToSelection(i);
                }
            }
        }

        private void TileLocationChanged(object sender, EventArgs e)
        {
            Positioned<IMapTile> item = (Positioned<IMapTile>)sender;
            int index = this.model.Map.FloatingTiles.IndexOf(item);

            var mapping = this.tileMapping[index];
            bool selected = this.view.SelectedItemsContains(mapping);

            this.RemoveTile(index);
            this.InsertTile(item, index);

            if (selected)
            {
                this.view.AddToSelection(this.tileMapping[index]);
            }
        }

        private void BaseTileChanged(object sender, EventArgs e)
        {
            this.view.Invalidate();
        }

        private void TilesChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    this.InsertTile(this.model.Map.FloatingTiles[e.NewIndex], e.NewIndex);
                    this.model.Map.FloatingTiles[e.NewIndex].LocationChanged += this.TileLocationChanged;
                    break;
                case ListChangedType.ItemDeleted:
                    this.RemoveTile(e.NewIndex);
                    break;
                case ListChangedType.ItemMoved:
                    this.RemoveTile(e.OldIndex);
                    this.InsertTile(this.model.Map.FloatingTiles[e.NewIndex], e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    this.PopulateView(); // probably a bit heavy-handed
                    break;
                default:
                    throw new ArgumentException("unknown list changed type: " + e.ListChangedType);
            }
        }

        private void FeatureChanged(object sender, SparseGridEventArgs e)
        {
            switch (e.Action)
            {
                case SparseGridEventArgs.ActionType.Set:
                    foreach (var index in e.Indexes)
                    {
                        this.UpdateFeature(this.ToFeaturePoint(index));
                    }

                    break;
                case SparseGridEventArgs.ActionType.Move:
                    var oldIter = e.OriginalIndexes.GetEnumerator();
                    var newIter = e.Indexes.GetEnumerator();
                    while (oldIter.MoveNext() && newIter.MoveNext())
                    {
                        this.MoveFeature(
                            this.ToFeaturePoint(oldIter.Current),
                            this.ToFeaturePoint(newIter.Current));
                    }

                    break;
                case SparseGridEventArgs.ActionType.Remove:
                    foreach (var index in e.Indexes)
                    {
                        this.RemoveFeature(this.ToFeaturePoint(index));
                    }

                    break;
            }
        }

        private void UpdateBandbox()
        {
            if (this.bandboxMapping != null)
            {
                this.view.Items.Remove(this.bandboxMapping);
            }

            if (this.selectionModel.BandboxRectangle != Rectangle.Empty)
            {
                var bandbox = DrawableBandbox.CreateSimple(
                    this.selectionModel.BandboxRectangle.Size,
                    BandboxFillColor,
                    BandboxBorderColor);

                this.bandboxMapping = new ImageLayerCollection.Item(
                    this.selectionModel.BandboxRectangle.X,
                    this.selectionModel.BandboxRectangle.Y,
                    BandboxDepth,
                    bandbox);

                this.bandboxMapping.Locked = true;

                this.view.Items.Add(this.bandboxMapping);
            }
        }
    }
}
