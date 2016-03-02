namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows.Forms;

    using Mappy.Data;
    using Mappy.UI.Controls;
    using Mappy.UI.Drawables;
    using Mappy.UI.Tags;
    using Collections;

    using Mappy.Maybe;
    using Mappy.Services;

    public class MapViewViewModel : IMapViewViewModel
    {
        private const int BandboxDepth = 100000000;

        private static readonly Color BandboxFillColor = Color.FromArgb(127, Color.Blue);

        private static readonly Color BandboxBorderColor = Color.FromArgb(127, Color.Black);

        private static readonly IDrawable[] StartPositionImages = LoadStartPositionImages();

        private static readonly Feature DefaultFeatureRecord = new Feature
        {
            Name = "default",
            Offset = new Point(0, 0),
            Footprint = new Size(1, 1),
            Image = Mappy.Properties.Resources.nofeature
        };

        private readonly List<DrawableItem> tileMapping = new List<DrawableItem>();

        private readonly IDictionary<Guid, DrawableItem> featureMapping =
            new Dictionary<Guid, DrawableItem>();

        private readonly DrawableItem[] startPositionMapping = new DrawableItem[10];

        private readonly GridLayer grid = new GridLayer(16, Color.Black);

        private readonly GuideLayer guides = new GuideLayer();

        private readonly BehaviorSubject<SelectableItemsLayer> itemsLayer =
            new BehaviorSubject<SelectableItemsLayer>(new SelectableItemsLayer(0, 0));

        private readonly IReadOnlyApplicationModel model;

        private readonly Dispatcher dispatcher;

        private readonly FeatureService featureService;

        private IMainModel mapModel;

        private bool mouseDown;

        private Point lastMousePos;

        private bool bandboxMode;

        private DrawableItem bandboxMapping;

        private DrawableTile baseTile;

        private DrawableItem baseItem;

        private bool featuresVisible;

        public MapViewViewModel(IReadOnlyApplicationModel model, Dispatcher dispatcher, FeatureService featureService)
        {
            var heightmapVisible = model.PropertyAsObservable(x => x.HeightmapVisible, nameof(model.HeightmapVisible));
            var gridVisible = model.PropertyAsObservable(x => x.GridVisible, nameof(model.GridVisible));
            var gridColor = model.PropertyAsObservable(x => x.GridColor, nameof(model.GridColor));
            var gridSize = model.PropertyAsObservable(x => x.GridSize, nameof(model.GridSize));
            var featuresVisible = model.PropertyAsObservable(x => x.FeaturesVisible, nameof(model.FeaturesVisible));
            var map = model.PropertyAsObservable(x => x.Map, nameof(model.Map));

            var mapWidth = map.ObservePropertyOrDefault(x => x.MapWidth, "MapWidth", 0);
            var mapHeight = map.ObservePropertyOrDefault(x => x.MapHeight, "MapHeight", 0);

            this.ViewportLocation = map.ObservePropertyOrDefault(
                x => x.ViewportLocation,
                "ViewportLocation",
                Point.Empty);

            map.Subscribe(this.SetMapModel);
            gridVisible.Subscribe(x => this.grid.Enabled = x);
            gridColor.Subscribe(x => this.grid.Color = x);

            // FIXME: this should not ignore height
            gridSize.Subscribe(x => this.grid.CellSize = x.Width);
            heightmapVisible.Subscribe(this.RefreshHeightmapVisibility);
            featuresVisible.Subscribe(x => this.featuresVisible = x);

            this.CanvasSize = mapWidth.CombineLatest(mapHeight, (w, h) => new Size(w * 32, h * 32));
            this.CanvasSize.Subscribe(
                x =>
                    {
                        this.guides.ClearGuides();
                        this.guides.AddHorizontalGuide(x.Height - 128);
                        this.guides.AddVerticalGuide(x.Width - 32);
                    });

            map.ObservePropertyOrDefault(x => x.SelectedTile, "SelectedTile", null)
                .Subscribe(_ => this.RefreshSelection());
            map.ObservePropertyOrDefault(x => x.SelectedStartPosition, "SelectedStartPosition", null)
                .Subscribe(_ => this.RefreshSelection());

            map
                .Where(x => x.IsSome)
                .Select(x => x.UnsafeValue) // will never be null due to where clause
                .Select(x => x.SelectedFeatures)
                .Subscribe(x => x.CollectionChanged += this.SelectedFeaturesCollectionChanged);

            this.model = model;
            this.dispatcher = dispatcher;
            this.featureService = featureService;
        }

        public IObservable<Size> CanvasSize { get; }

        public IObservable<Point> ViewportLocation { get; }

        public IObservable<ILayer> ItemsLayer => this.itemsLayer;

        public ILayer GridLayer => this.grid;

        public ILayer GuidesLayer => this.guides;

        public void DragDrop(IDataObject data, Point location)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])data.GetData(DataFormats.FileDrop);
                if (files.Length < 1)
                {
                    return;
                }

                this.dispatcher.OpenFromDragDrop(files[0]);
            }
            else if (data.GetDataPresent(typeof(StartPositionDragData)))
            {
                StartPositionDragData posData = (StartPositionDragData)data.GetData(typeof(StartPositionDragData));
                this.dispatcher.SetStartPosition(posData.PositionNumber, location.X, location.Y);
            }
            else if (data.GetDataPresent(DataFormats.Text))
            {
                var dataString = (string)data.GetData(DataFormats.Text);
                int id;
                if (int.TryParse(dataString, out id))
                {
                    this.dispatcher.DragDropSection(id, location.X, location.Y);
                }
                else
                {
                    this.dispatcher.DragDropFeature(dataString, location.X, location.Y);
                }
            }
        }

        public void ClientSizeChanged(Size size)
        {
            this.dispatcher.SetViewportSize(size);
        }

        public void ScrollPositionChanged(Point position)
        {
            this.dispatcher.SetViewportLocation(position);
        }

        public void MouseDown(Point location)
        {
            this.mouseDown = true;
            this.lastMousePos = location;

            if (!this.itemsLayer.Value.IsInSelection(location.X, location.Y))
            {
                var hit = this.itemsLayer.Value.HitTest(location.X, location.Y);
                if (hit != null)
                {
                    this.SelectFromTag(hit.Tag);
                }
                else
                {
                    this.dispatcher.ClearSelection();
                    this.dispatcher.StartBandbox(location.X, location.Y);
                    this.bandboxMode = true;
                }
            }
        }

        public void MouseMove(Point location)
        {
            try
            {
                if (!this.mouseDown)
                {
                    return;
                }

                if (this.bandboxMode)
                {
                    this.dispatcher.GrowBandbox(
                        location.X - this.lastMousePos.X,
                        location.Y - this.lastMousePos.Y);
                }
                else
                {
                    this.dispatcher.TranslateSelection(
                        location.X - this.lastMousePos.X,
                        location.Y - this.lastMousePos.Y);
                }
            }
            finally
            {
                this.lastMousePos = location;
            }
        }

        public void MouseUp()
        {
            this.mouseDown = false;

            if (this.bandboxMode)
            {
                this.dispatcher.CommitBandbox();
                this.bandboxMode = false;
            }
            else
            {
                this.dispatcher.FlushTranslation();
            }
        }

        public void KeyDown(Keys key)
        {
            if (key == Keys.Delete)
            {
                this.dispatcher.DeleteSelection();
            }
        }

        public void LeaveFocus()
        {
            this.dispatcher.ClearSelection();
        }

        private static IDrawable[] LoadStartPositionImages()
        {
            var arr = new IDrawable[10];
            for (int i = 0; i < 10; i++)
            {
                var image = new DrawableBitmap(Mappy.Util.Util.GetStartImage(i + 1));
                arr[i] = image;
            }

            return arr;
        }

        private void SetMapModel(Maybe<UndoableMapModel> model)
        {
            this.mapModel = model.Or(null);
            this.WireMapModel();
            this.ResetView();
        }

        private void ResetView()
        {
            this.UpdateItemsLayer();

            this.UpdateBaseTile();

            this.UpdateFloatingTiles();

            this.UpdateFeatures();

            this.UpdateStartPositions();
        }

        private void UpdateItemsLayer()
        {
            if (this.mapModel == null)
            {
                this.itemsLayer.OnNext(new SelectableItemsLayer(0, 0));
            }
            else
            {
                this.itemsLayer.OnNext(
                    new SelectableItemsLayer(
                        this.mapModel.MapWidth * 32,
                        this.mapModel.MapHeight * 32));
            }
        }

        private void UpdateStartPositions()
        {
            for (int i = 0; i < 10; i++)
            {
                this.UpdateStartPosition(i);
            }
        }

        private void UpdateFeatures()
        {
            foreach (var f in this.featureMapping.Values)
            {
                this.itemsLayer.Value.Items.Remove(f);
            }

            this.featureMapping.Clear();

            if (this.mapModel == null)
            {
                return;
            }

            foreach (var f in this.mapModel.EnumerateFeatureInstances())
            {
                this.InsertFeature(f.Id);
            }
        }

        private void UpdateBaseTile()
        {
            if (this.baseItem != null)
            {
                this.itemsLayer.Value.Items.Remove(this.baseItem);
            }

            if (this.mapModel == null)
            {
                this.baseTile = null;
                this.baseItem = null;
                return;
            }

            this.baseTile = new DrawableTile(this.mapModel.BaseTile);
            this.baseTile.BackgroundColor = Color.CornflowerBlue;
            this.baseTile.DrawHeightMap = this.model.HeightmapVisible;
            this.baseTile.SeaLevel = this.mapModel.SeaLevel;
            this.baseItem = new DrawableItem(
                0,
                0,
                -1,
                this.baseTile);

            this.baseItem.Locked = true;

            this.itemsLayer.Value.Items.Add(this.baseItem);
        }

        private void RefreshHeightmapVisibility(bool visible)
        {
            if (this.baseTile == null)
            {
                return;
            }

            this.baseTile.DrawHeightMap = visible;
        }

        private void RefreshSeaLevel()
        {
            this.baseTile.SeaLevel = this.mapModel.SeaLevel;
        }

        private void WireMapModel()
        {
            if (this.mapModel == null)
            {
                return;
            }

            this.mapModel.TilesChanged += this.TilesChanged;
            this.mapModel.BaseTileGraphicsChanged += this.BaseTileChanged;
            this.mapModel.BaseTileHeightChanged += this.BaseTileChanged;

            foreach (var t in this.mapModel.FloatingTiles)
            {
                t.LocationChanged += this.TileLocationChanged;
            }

            this.mapModel.FeatureInstanceChanged += this.FeatureInstanceChanged;

            this.mapModel.StartPositionChanged += this.StartPositionChanged;

            this.mapModel.PropertyChanged += this.MapModelPropertyChanged;
        }

        private void SelectedFeaturesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RefreshSelection();
        }

        private void MapModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "SeaLevel":
                    this.RefreshSeaLevel();
                    break;
                case "BandboxRectangle":
                    this.UpdateBandbox();
                    break;
            }
        }

        private void UpdateBandbox()
        {
            if (this.bandboxMapping != null)
            {
                this.itemsLayer.Value.Items.Remove(this.bandboxMapping);
            }

            if (this.mapModel == null)
            {
                return;
            }

            if (this.mapModel.BandboxRectangle == Rectangle.Empty)
            {
                return;
            }

            var bandbox = DrawableBandbox.CreateSimple(
                this.mapModel.BandboxRectangle.Size,
                BandboxFillColor,
                BandboxBorderColor);

            this.bandboxMapping = new DrawableItem(
                this.mapModel.BandboxRectangle.X,
                this.mapModel.BandboxRectangle.Y,
                BandboxDepth,
                bandbox);

            this.bandboxMapping.Locked = true;

            this.itemsLayer.Value.Items.Add(this.bandboxMapping);
        }

        private void RefreshSelection()
        {
            this.itemsLayer.Value.ClearSelection();

            if (this.mapModel == null)
            {
                return;
            }

            if (this.mapModel.SelectedTile.HasValue)
            {
                if (this.tileMapping.Count > this.mapModel.SelectedTile)
                {
                    this.itemsLayer.Value.AddToSelection(this.tileMapping[this.mapModel.SelectedTile.Value]);
                }
            }
            else if (this.mapModel.SelectedFeatures.Count > 0)
            {
                foreach (var item in this.mapModel.SelectedFeatures)
                {
                    if (this.featureMapping.ContainsKey(item))
                    {
                        this.itemsLayer.Value.AddToSelection(this.featureMapping[item]);
                    }
                }
            }
            else if (this.mapModel.SelectedStartPosition.HasValue)
            {
                var mapping = this.startPositionMapping[this.mapModel.SelectedStartPosition.Value];
                if (mapping != null)
                {
                    this.itemsLayer.Value.AddToSelection(mapping);
                }
            }
        }

        private void BaseTileChanged(object sender, EventArgs e)
        {
            this.baseTile.Invalidate();
        }

        private void TileLocationChanged(object sender, EventArgs e)
        {
            Positioned<IMapTile> item = (Positioned<IMapTile>)sender;
            int index = this.mapModel.FloatingTiles.IndexOf(item);

            this.RemoveTile(index);
            this.InsertTile(item, index);
        }

        private void StartPositionChanged(object sender, StartPositionChangedEventArgs e)
        {
            this.UpdateStartPosition(e.Index);
        }

        private void UpdateStartPosition(int index)
        {
            if (this.startPositionMapping[index] != null)
            {
                var mapping = this.startPositionMapping[index];
                this.itemsLayer.Value.Items.Remove(mapping);
                this.itemsLayer.Value.RemoveFromSelection(mapping);
                this.startPositionMapping[index] = null;
            }

            if (this.mapModel == null)
            {
                return;
            }

            Point? p = this.mapModel.GetStartPosition(index);
            if (p.HasValue)
            {
                IDrawable img = StartPositionImages[index];
                var i = new DrawableItem(
                    p.Value.X - (img.Width / 2),
                    p.Value.Y - 58,
                    int.MaxValue,
                    img);
                i.Tag = new StartPositionTag(index);
                this.startPositionMapping[index] = i;
                this.itemsLayer.Value.Items.Add(i);

                if (this.mapModel.SelectedStartPosition == index)
                {
                    this.itemsLayer.Value.AddToSelection(i);
                }
            }
        }

        private void TilesChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    this.InsertTile(this.mapModel.FloatingTiles[e.NewIndex], e.NewIndex);
                    this.mapModel.FloatingTiles[e.NewIndex].LocationChanged += this.TileLocationChanged;
                    break;
                case ListChangedType.ItemDeleted:
                    this.RemoveTile(e.NewIndex);
                    break;
                case ListChangedType.ItemMoved:
                    this.RemoveTile(e.OldIndex);
                    this.InsertTile(this.mapModel.FloatingTiles[e.NewIndex], e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    this.UpdateFloatingTiles();
                    break;
                default:
                    throw new ArgumentException("unknown list changed type: " + e.ListChangedType);
            }
        }

        private void FeatureInstanceChanged(object sender, FeatureInstanceEventArgs e)
        {
            switch (e.Action)
            {
                case FeatureInstanceEventArgs.ActionType.Add:
                    this.InsertFeature(e.FeatureInstanceId);
                    break;
                case FeatureInstanceEventArgs.ActionType.Move:
                    this.UpdateFeature(e.FeatureInstanceId);
                    break;
                case FeatureInstanceEventArgs.ActionType.Remove:
                    this.RemoveFeature(e.FeatureInstanceId);
                    break;
            }
        }

        private void UpdateFloatingTiles()
        {
            foreach (var t in this.tileMapping)
            {
                this.itemsLayer.Value.Items.Remove(t);
            }

            this.tileMapping.Clear();

            if (this.mapModel == null)
            {
                return;
            }

            int count = 0;
            foreach (var t in this.mapModel.FloatingTiles)
            {
                this.InsertTile(t, count++);
            }
        }

        private void InsertTile(Positioned<IMapTile> t, int index)
        {
            var drawable = new DrawableTile(t.Item);
            drawable.BackgroundColor = Color.CornflowerBlue;
            DrawableItem i = new DrawableItem(
                    t.Location.X * 32,
                    t.Location.Y * 32,
                    index,
                    drawable);
            i.Tag = new SectionTag(index);
            this.tileMapping.Insert(index, i);
            this.itemsLayer.Value.Items.Add(i);

            if (this.mapModel.SelectedTile == index)
            {
                this.itemsLayer.Value.AddToSelection(i);
            }
        }

        private void RemoveTile(int index)
        {
            DrawableItem item = this.tileMapping[index];
            this.itemsLayer.Value.Items.Remove(item);
            this.itemsLayer.Value.RemoveFromSelection(item);
            this.tileMapping.RemoveAt(index);
        }

        private int ToFeatureIndex(GridCoordinates p)
        {
            return this.ToFeatureIndex(p.X, p.Y);
        }

        private int ToFeatureIndex(int x, int y)
        {
            return (y * this.mapModel.FeatureGridWidth) + x;
        }

        private void InsertFeature(Guid id)
        {
            var f = this.mapModel.GetFeatureInstance(id);
            var coords = f.Location;
            int index = this.ToFeatureIndex(coords);

            var featureRecord = this.featureService.TryGetFeature(f.FeatureName).Or(DefaultFeatureRecord);

            Rectangle r = featureRecord.GetDrawBounds(this.mapModel.BaseTile.HeightGrid, coords.X, coords.Y);
            DrawableItem i = new DrawableItem(
                    r.X,
                    r.Y,
                    index + 1000, // magic number to separate from tiles
                    new DrawableBitmap(featureRecord.Image));
            i.Tag = new FeatureTag(f.Id);
            i.Visible = this.featuresVisible;
            this.featureMapping[f.Id] = i;
            this.itemsLayer.Value.Items.Add(i);

            if (this.mapModel.SelectedFeatures.Contains(f.Id))
            {
                this.itemsLayer.Value.AddToSelection(i);
            }
        }

        private void UpdateFeature(Guid id)
        {
            this.RemoveFeature(id);
            this.InsertFeature(id);
        }

        private void RemoveFeature(Guid id)
        {
            if (this.featureMapping.ContainsKey(id))
            {
                DrawableItem item = this.featureMapping[id];
                this.itemsLayer.Value.Items.Remove(item);
                this.itemsLayer.Value.RemoveFromSelection(item);
                this.featureMapping.Remove(id);
            }
        }

        private void SelectFromTag(object tag)
        {
            IMapItemTag t = (IMapItemTag)tag;
            t.SelectItem(this.dispatcher);
        }
    }
}
