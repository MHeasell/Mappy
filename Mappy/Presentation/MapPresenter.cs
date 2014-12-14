namespace Mappy.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Collections;
    using Mappy.Controllers.Tags;
    using Mappy.Data;
    using Mappy.Models;
    using Mappy.UI.Controls;
    using Mappy.UI.Drawables;
    using Mappy.Util;

    public class MapPresenter
    {
        private const int BandboxDepth = 100000000;

        private static readonly Color BandboxFillColor = Color.FromArgb(127, Color.Blue);

        private static readonly Color BandboxBorderColor = Color.FromArgb(127, Color.Black);

        private static readonly IDrawable[] StartPositionImages = new IDrawable[10];

        private static readonly Feature DefaultFeatureRecord = new Feature
            {
                Name = "default",
                Offset = new Point(0, 0),
                Footprint = new Size(1, 1),
                Image = Mappy.Properties.Resources.nofeature
            };

        private readonly IMapView view;

        private readonly IMainModel model;

        private readonly List<ImageLayerCollection.Item> tileMapping = new List<ImageLayerCollection.Item>();

        private readonly IDictionary<Guid, ImageLayerCollection.Item> featureMapping =
            new Dictionary<Guid, ImageLayerCollection.Item>();

        private readonly ImageLayerCollection.Item[] startPositionMapping = new ImageLayerCollection.Item[10];

        private DrawableTile baseTile;

        private ImageLayerCollection.Item bandboxMapping;

        private bool mouseDown;

        private Point lastMousePos;

        private bool bandboxMode;

        private ImageLayerCollection.Item baseItem;

        static MapPresenter()
        {
            for (int i = 0; i < 10; i++)
            {
                var image = new DrawableBitmap(Util.GetStartImage(i + 1));
                MapPresenter.StartPositionImages[i] = image;
            }
        }

        public MapPresenter(IMapView view, IMainModel model)
        {
            this.view = view;
            this.model = model;

            this.model.PropertyChanged += this.ModelPropertyChanged;

            this.PopulateView();

            this.WireMap();

            if (this.model.Map != null)
            {
                this.WireInnerMap();
            }

            this.view.GridVisible = this.model.GridVisible;
            this.view.GridColor = this.model.GridColor;
            this.view.GridSize = this.model.GridSize;
        }

        public void DragDrop(IDataObject data, int virtualX, int virtualY)
        {
            if (!this.model.MapOpen)
            {
                return;
            }

            if (data.GetDataPresent(typeof(StartPositionDragData)))
            {
                StartPositionDragData posData = (StartPositionDragData)data.GetData(typeof(StartPositionDragData));
                this.model.DragDropStartPosition(posData.PositionNumber, virtualX, virtualY);
            }
            else
            {
                string dataString = data.GetData(DataFormats.Text).ToString();
                int id;
                if (int.TryParse(dataString, out id))
                {
                    this.model.DragDropTile(id, virtualX, virtualY);
                }
                else
                {
                    this.model.DragDropFeature(dataString, virtualX, virtualY);
                }
            }
        }

        public void MouseDown(int virtualX, int virtualY)
        {
            this.mouseDown = true;
            this.lastMousePos = new Point(virtualX, virtualY);

            if (!this.model.MapOpen)
            {
                return;
            }

            if (!this.view.IsInSelection(virtualX, virtualY))
            {
                var hit = this.view.HitTest(virtualX, virtualY);
                if (hit != null)
                {
                    this.SelectFromTag(hit.Tag);
                }
                else
                {
                    this.model.ClearSelection();
                    this.model.StartBandbox(virtualX, virtualY);
                    this.bandboxMode = true;
                }
            }
        }

        public void MouseMove(int virtualX, int virtualY)
        {
            try
            {
                if (!this.model.MapOpen)
                {
                    return;
                }

                if (!this.mouseDown)
                {
                    return;
                }

                if (this.bandboxMode)
                {
                    this.model.GrowBandbox(
                        virtualX - this.lastMousePos.X,
                        virtualY - this.lastMousePos.Y);
                }
                else
                {
                    this.model.TranslateSelection(
                        virtualX - this.lastMousePos.X,
                        virtualY - this.lastMousePos.Y);
                }
            }
            finally
            {
                this.lastMousePos = new Point(virtualX, virtualY);
            }
        }

        public void MouseUp(int virtualX, int virtualY)
        {
            this.mouseDown = false;

            if (!this.model.MapOpen)
            {
                return;
            }

            if (this.bandboxMode)
            {
                this.model.CommitBandbox();
                this.bandboxMode = false;
            }
            else
            {
                this.model.FlushTranslation();
            }
        }

        public void KeyDown(KeyEventArgs e)
        {
            if (!this.model.MapOpen)
            {
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                this.model.DeleteSelection();
            }
        }

        public void LostFocus()
        {
            if (!this.model.MapOpen)
            {
                return;
            }

            this.model.ClearSelection();
        }

        private void SelectFromTag(object tag)
        {
            IMapItemTag t = (IMapItemTag)tag;
            t.SelectItem(this.model);
        }

        private void WireInnerMap()
        {
            this.model.Map.FeatureInstanceChanged += this.FeatureInstanceChanged;
        }

        private void WireMap()
        {
            this.model.TilesChanged += this.TilesChanged;

            this.model.BaseTileGraphicsChanged += this.BaseTileChanged;
            this.model.BaseTileHeightChanged += this.BaseTileChanged;

            if (this.model.FloatingTiles != null)
            {
                foreach (var t in this.model.FloatingTiles)
                {
                    t.LocationChanged += this.TileLocationChanged;
                }
            }

            this.model.StartPositionChanged += this.StartPositionChanged;
        }

        private void UpdateBaseTile()
        {
            if (this.baseItem != null)
            {
                this.view.Items.Remove(this.baseItem);
            }

            if (!this.model.MapOpen)
            {
                this.baseTile = null;
                this.baseItem = null;
                return;
            }

            this.baseTile = new DrawableTile(this.model.BaseTile);
            this.baseTile.BackgroundColor = Color.CornflowerBlue;
            this.baseTile.DrawHeightMap = this.model.HeightmapVisible;
            this.baseTile.SeaLevel = this.model.SeaLevel;
            this.baseItem = new ImageLayerCollection.Item(
                0,
                0,
                -1,
                this.baseTile);

            this.baseItem.Locked = true;

            this.view.Items.Add(this.baseItem);
        }

        private void UpdateCanvasSize()
        {
            if (!this.model.MapOpen)
            {
                this.view.CanvasSize = Size.Empty;
                return;
            }

            this.view.CanvasSize = new Size(
                this.model.MapWidth * 32,
                this.model.MapHeight * 32);
        }

        private void UpdateFloatingTiles()
        {
            foreach (var t in this.tileMapping)
            {
                this.view.Items.Remove(t);
            }

            this.tileMapping.Clear();

            if (!this.model.MapOpen)
            {
                return;
            }

            int count = 0;
            foreach (var t in this.model.FloatingTiles)
            {
                this.InsertTile(t, count++);
            }
        }

        private void UpdateFeatures()
        {
            foreach (var f in this.featureMapping.Values)
            {
                this.view.Items.Remove(f);
            }

            this.featureMapping.Clear();

            if (!this.model.MapOpen)
            {
                return;
            }

            foreach (var f in this.model.Map.EnumerateFeatureInstances())
            {
                this.InsertFeature(f.Id);
            }
        }

        private void PopulateView()
        {
            foreach (var item in this.featureMapping.Values)
            {
                this.view.Items.Remove(item);
            }

            this.featureMapping.Clear();

            this.UpdateCanvasSize();
            this.UpdateBaseTile();

            this.UpdateFloatingTiles();

            this.UpdateFeatures();
        }

        private void InsertTile(Positioned<IMapTile> t, int index)
        {
            var drawable = new DrawableTile(t.Item);
            drawable.BackgroundColor = Color.CornflowerBlue;
            ImageLayerCollection.Item i = new ImageLayerCollection.Item(
                    t.Location.X * 32,
                    t.Location.Y * 32,
                    index,
                    drawable);
            i.Tag = new SectionTag(index);
            this.tileMapping.Insert(index, i);
            this.view.Items.Add(i);

            if (this.model.SelectedTile == index)
            {
                this.view.AddToSelection(i);
            }
        }

        private void RemoveTile(int index)
        {
            ImageLayerCollection.Item item = this.tileMapping[index];
            this.view.Items.Remove(item);
            this.view.RemoveFromSelection(item);
            this.tileMapping.RemoveAt(index);
        }

        private void InsertFeature(Guid id)
        {
            var f = this.model.Map.GetFeatureInstance(id);
            var coords = f.Location;
            int index = this.ToFeatureIndex(coords);
            Feature featureRecord;
            if (!this.model.FeatureRecords.TryGetFeature(f.FeatureName, out featureRecord))
            {
                featureRecord = DefaultFeatureRecord;
            }

            Rectangle r = featureRecord.GetDrawBounds(this.model.BaseTile.HeightGrid, coords.X, coords.Y);
            ImageLayerCollection.Item i = new ImageLayerCollection.Item(
                    r.X,
                    r.Y,
                    index + 1000, // magic number to separate from tiles
                    new DrawableBitmap(featureRecord.Image));
            i.Tag = new FeatureTag(f.Id);
            i.Visible = this.model.FeaturesVisible;
            this.featureMapping[f.Id] = i;
            this.view.Items.Add(i);

            if (this.model.Map.SelectedFeatures.Contains(f.Id))
            {
                this.view.AddToSelection(i);
            }
        }

        private bool RemoveFeature(Guid id)
        {
            if (this.featureMapping.ContainsKey(id))
            {
                ImageLayerCollection.Item item = this.featureMapping[id];
                this.view.Items.Remove(item);
                this.view.RemoveFromSelection(item);
                this.featureMapping.Remove(id);
                return true;
            }

            return false;
        }

        private void UpdateFeature(Guid id)
        {
            this.RemoveFeature(id);
            this.InsertFeature(id);
        }

        private GridCoordinates ToFeaturePoint(int index)
        {
            int x = index % this.model.Map.FeatureGridWidth;
            int y = index / this.model.Map.FeatureGridHeight;
            var p = new GridCoordinates(x, y);
            return p;
        }

        private int ToFeatureIndex(GridCoordinates p)
        {
            return this.ToFeatureIndex(p.X, p.Y);
        }

        private int ToFeatureIndex(int x, int y)
        {
            return (y * this.model.Map.FeatureGridWidth) + x;
        }

        private void RefreshFeatureVisibility()
        {
            foreach (var i in this.featureMapping.Values)
            {
                i.Visible = this.model.FeaturesVisible;
            }
        }

        private void RefreshHeightmapVisibility()
        {
            if (this.baseTile == null)
            {
                return;
            }

            this.baseTile.DrawHeightMap = this.model.HeightmapVisible;
            this.view.Invalidate();
        }

        private void RefreshSeaLevel()
        {
            if (this.baseTile == null)
            {
                return;
            }

            this.baseTile.SeaLevel = this.model.SeaLevel;
            this.view.Invalidate();
        }

        private void RefreshSelection()
        {
            this.view.ClearSelection();

            if (!this.model.MapOpen)
            {
                return;
            }

            if (this.model.SelectedTile.HasValue)
            {
                if (this.tileMapping.Count > this.model.SelectedTile)
                {
                    this.view.AddToSelection(this.tileMapping[this.model.SelectedTile.Value]);
                }
            }
            else if (this.model.Map.SelectedFeatures.Count > 0)
            {
                foreach (var item in this.model.Map.SelectedFeatures)
                {
                    if (this.featureMapping.ContainsKey(item))
                    {
                        this.view.AddToSelection(this.featureMapping[item]);
                    }
                }
            }
            else if (this.model.SelectedStartPosition.HasValue)
            {
                var mapping = this.startPositionMapping[this.model.SelectedStartPosition.Value];
                if (mapping != null)
                {
                    this.view.AddToSelection(mapping);
                }
            }
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
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
                    this.view.GridVisible = this.model.GridVisible;
                    break;
                case "GridColor":
                    this.view.GridColor = this.model.GridColor;
                    break;
                case "GridSize":
                    this.view.GridSize = this.model.GridSize;
                    break;

                case "SelectedTile":
                case "SelectedFeatures":
                case "SelectedStartPosition":
                    this.RefreshSelection();
                    break;
                case "BandboxRectangle":
                    this.UpdateBandbox();
                    break;

                case "BaseTile":
                    this.UpdateBaseTile();
                    break;
                case "MapWidth":
                case "MapHeight":
                    this.UpdateCanvasSize();
                    break;
                case "FloatingTiles":
                    this.UpdateFloatingTiles();
                    break;
                case "Features":
                    this.UpdateFeatures();
                    break;
                case "SeaLevel":
                    this.RefreshSeaLevel();
                    break;
                case "Map":
                    if (this.model.Map != null)
                    {
                        this.WireInnerMap();
                    }

                    break;
            }
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
                this.view.Items.Remove(mapping);
                this.view.RemoveFromSelection(mapping);
                this.startPositionMapping[index] = null;
            }

            Point? p = this.model.GetStartPosition(index);
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

                if (this.model.SelectedStartPosition == index)
                {
                    this.view.AddToSelection(i);
                }
            }
        }

        private void TileLocationChanged(object sender, EventArgs e)
        {
            Positioned<IMapTile> item = (Positioned<IMapTile>)sender;
            int index = this.model.FloatingTiles.IndexOf(item);

            this.RemoveTile(index);
            this.InsertTile(item, index);
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
                    this.InsertTile(this.model.FloatingTiles[e.NewIndex], e.NewIndex);
                    this.model.FloatingTiles[e.NewIndex].LocationChanged += this.TileLocationChanged;
                    break;
                case ListChangedType.ItemDeleted:
                    this.RemoveTile(e.NewIndex);
                    break;
                case ListChangedType.ItemMoved:
                    this.RemoveTile(e.OldIndex);
                    this.InsertTile(this.model.FloatingTiles[e.NewIndex], e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    this.UpdateFloatingTiles();
                    this.PopulateView(); // probably a bit heavy-handed
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

        private void UpdateBandbox()
        {
            if (this.bandboxMapping != null)
            {
                this.view.Items.Remove(this.bandboxMapping);
            }

            if (this.model.BandboxRectangle != Rectangle.Empty)
            {
                var bandbox = DrawableBandbox.CreateSimple(
                    this.model.BandboxRectangle.Size,
                    BandboxFillColor,
                    BandboxBorderColor);

                this.bandboxMapping = new ImageLayerCollection.Item(
                    this.model.BandboxRectangle.X,
                    this.model.BandboxRectangle.Y,
                    BandboxDepth,
                    bandbox);

                this.bandboxMapping.Locked = true;

                this.view.Items.Add(this.bandboxMapping);
            }
        }
    }
}
