namespace Mappy.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Data;

    using Mappy.Collections;

    using Models;
    using UI.Controls;
    using UI.Drawables;
    using Util;

    public class MapPresenter
    {
        private static readonly IDrawable[] StartPositionImages = new IDrawable[10];

        private readonly ImageLayerView view;
        private readonly CoreModel model;

        private readonly List<ImageLayerCollection.Item> tileMapping = new List<ImageLayerCollection.Item>();
        private readonly IDictionary<int, ImageLayerCollection.Item> featureMapping = new Dictionary<int, ImageLayerCollection.Item>();

        private readonly ImageLayerCollection.Item[] startPositionMapping = new ImageLayerCollection.Item[10];

        private bool mouseDown;
        private Point lastMousePos;
        private Point delta;

        private DrawableTile baseTile;

        static MapPresenter()
        {
            for (int i = 0; i < 10; i++)
            {
                var image = new DrawableBitmap(Util.GetStartImage(i + 1));
                MapPresenter.StartPositionImages[i] = image;
            }
        }

        public MapPresenter(ImageLayerView view, CoreModel model)
        {
            this.view = view;
            this.model = model;

            this.model.PropertyChanged += this.ModelPropertyChanged;

            this.PopulateView();
            this.WireMap();

            this.view.MouseDown += this.ViewMouseDown;
            this.view.MouseMove += this.ViewMouseMove;
            this.view.MouseUp += this.ViewMouseUp;
            this.view.KeyDown += this.ViewKeyDown;

            this.view.DragEnter += this.ViewDragEnter;
            this.view.DragDrop += this.ViewDragDrop;

            this.view.GridVisible = this.model.GridVisible;
            this.view.GridColor = this.model.GridColor;
            this.view.GridSize = this.model.GridSize;
        }

        public void DeleteSelection()
        {
            if (this.view.SelectedItem == null)
            {
                return;
            }

            FeatureTag f = this.view.SelectedItem.Tag as FeatureTag;
            if (f != null)
            {
                this.model.RemoveFeature(f.Coordinates);
            }
            else
            {
                Positioned<IMapTile> t = this.view.SelectedItem.Tag as Positioned<IMapTile>;
                if (t != null)
                {
                    int index = this.model.Map.FloatingTiles.IndexOf(t);
                    this.model.RemoveSection(index);
                }
                else
                {
                    StartPositionTag st = this.view.SelectedItem.Tag as StartPositionTag;
                    if (st != null)
                    {
                        this.model.RemoveStartPosition(st.Index);
                    }
                }
            }
        }

        private void ViewDragDrop(object sender, DragEventArgs e)
        {
            if (this.model.Map == null)
            {
                return;
            }

            Point pos = this.view.PointToClient(new Point(e.X, e.Y));
            pos = this.view.ToVirtualPoint(pos);

            if (e.Data.GetDataPresent(typeof(StartPositionDragData)))
            {
                StartPositionDragData posData = (StartPositionDragData)e.Data.GetData(typeof(StartPositionDragData));
                this.model.SetStartPosition(posData.PositionNumber, pos.X, pos.Y);
            }
            else
            {
                string data = e.Data.GetData(DataFormats.Text).ToString();
                int id;
                if (int.TryParse(data, out id))
                {
                    int quantX = pos.X / 32;
                    int quantY = pos.Y / 32;
                    this.model.PlaceSection(id, quantX, quantY);
                }
                else
                {
                    Point? featurePos = Util.ScreenToHeightIndex(this.model.Map.Tile.HeightGrid, pos);
                    if (featurePos.HasValue)
                    {
                        this.model.TryPlaceFeature(data, featurePos.Value.X, featurePos.Value.Y);
                    }
                }
            }
        }

        private void ViewDragEnter(object sender, DragEventArgs e)
        {
            if (this.model.Map == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(StartPositionDragData)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        #region Private Methods

        private void WireMap()
        {
            if (this.model.Map == null)
            {
                return;
            }

            this.model.Map.Features.EntriesChanged += this.FeatureChanged;
            this.model.Map.FloatingTiles.ListChanged += this.TilesChanged;

            this.model.Map.Tile.TileGrid.CellsChanged += this.BaseTileChanged;
            this.model.Map.Tile.HeightGrid.CellsChanged += this.BaseTileChanged;

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
            this.baseTile.DrawHeightMap = this.model.HeightmapVisible;
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

            foreach (var f in this.model.Map.Features.Entries)
            {
                this.InsertFeature(f.Value, f.X, f.Y);
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
            i.Tag = t;
            this.tileMapping.Insert(index, i);
            this.view.Items.Add(i);
        }

        private void RemoveTile(int index)
        {
            ImageLayerCollection.Item item = this.tileMapping[index];
            this.view.Items.Remove(item);
            this.tileMapping.RemoveAt(index);
            if (this.view.SelectedItem == item)
            {
                this.view.SelectedItem = null;
            }
        }

        private void InsertFeature(Feature f, int x, int y)
        {
            Rectangle r = f.GetDrawBounds(this.model.Map.Tile.HeightGrid, x, y);
            ImageLayerCollection.Item i = new ImageLayerCollection.Item(
                    r.X,
                    r.Y,
                    (y * this.model.Map.Features.Width) + x + 1000, // magic number to separate from tiles
                    new DrawableBitmap(f.Image));
            i.Tag = new FeatureTag { Coordinates = new Point(x, y) };
            i.Visible = this.model.FeaturesVisible;
            this.featureMapping[this.ToFeatureIndex(x, y)] = i;
            this.view.Items.Add(i);
        }

        private bool RemoveFeature(Point p)
        {
            int index = this.ToFeatureIndex(p);
            if (this.featureMapping.ContainsKey(index))
            {
                ImageLayerCollection.Item item = this.featureMapping[index];
                this.view.Items.Remove(item);
                this.featureMapping.Remove(index);
                if (this.view.SelectedItem == item)
                {
                    this.view.SelectedItem = null;
                }

                return true;
            }

            return false;
        }

        private void UpdateFeature(Point p)
        {
            this.RemoveFeature(p);
            Feature f;
            if (this.model.Map.Features.TryGetValue(p.X, p.Y, out f))
            {
                this.InsertFeature(f, p.X, p.Y);
            }
        }

        private Point ToFeaturePoint(int index)
        {
            int x = index % this.model.Map.Features.Width;
            int y = index / this.model.Map.Features.Width;
            Point p = new Point(x, y);
            return p;
        }

        private int ToFeatureIndex(Point p)
        {
            return this.ToFeatureIndex(p.X, p.Y);
        }

        private int ToFeatureIndex(int x, int y)
        {
            return (y * this.model.Map.Features.Width) + x;
        }

        #endregion

        private void DragFeatureTo(Point featureCoords, Point location)
        {
            Point? pos = Util.ScreenToHeightIndex(
                    this.model.Map.Tile.HeightGrid,
                    location);
            if (!pos.HasValue)
            {
                return;
            }

            bool success = this.model.TranslateFeature(
                featureCoords,
                pos.Value.X - featureCoords.X,
                pos.Value.Y - featureCoords.Y);

            if (success)
            {
                this.view.SelectedItem = this.featureMapping[this.ToFeatureIndex(pos.Value)];
            }
        }

        private void DragSectionTo(Positioned<IMapTile> t, Point location)
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

            int tileIndex = this.model.Map.FloatingTiles.IndexOf(t);
            this.view.SelectedItem = this.tileMapping[tileIndex];
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

        #region Model Event Handlers

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Map":
                    this.WireMap();
                    this.PopulateView();
                    this.RefreshHeightmapVisibility();
                    break;
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
                this.view.Items.Remove(this.startPositionMapping[index]);

                if (this.view.SelectedItem == this.startPositionMapping[index])
                {
                    this.view.SelectedItem = null;
                }

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
                i.Tag = new StartPositionTag { Index = index };
                this.startPositionMapping[index] = i;
                this.view.Items.Add(i);
            }
        }

        private void TileLocationChanged(object sender, EventArgs e)
        {
            Positioned<IMapTile> item = (Positioned<IMapTile>)sender;
            int index = this.model.Map.FloatingTiles.IndexOf(item);

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
                    foreach (var p in e.Indexes.Select(this.ToFeaturePoint))
                    {
                        this.UpdateFeature(p);
                    }

                    break;
                case SparseGridEventArgs.ActionType.Remove:
                    foreach (var p in e.Indexes.Select(this.ToFeaturePoint))
                    {
                        this.RemoveFeature(p);
                    }

                    break;
            }
        }

        #endregion

        #region View Event Handlers

        private void ViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.DeleteSelection();
            }
        }

        private void ViewMouseDown(object sender, MouseEventArgs e)
        {
            this.view.Focus();
            this.mouseDown = true;
            this.lastMousePos = this.view.ToVirtualPoint(e.Location);
            this.delta = new Point();
        }

        private void ViewMouseMove(object sender, MouseEventArgs e)
        {
            Point virtualLocation = this.view.ToVirtualPoint(e.Location);
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

                FeatureTag f = this.view.SelectedItem.Tag as FeatureTag;

                if (f != null)
                {
                    this.DragFeatureTo(f.Coordinates, virtualLocation);
                }
                else
                {
                    Positioned<IMapTile> t = this.view.SelectedItem.Tag as Positioned<IMapTile>;
                    if (t != null)
                    {
                        this.DragSectionTo(t, virtualLocation);
                    }
                    else
                    {
                        StartPositionTag s = this.view.SelectedItem.Tag as StartPositionTag;
                        if (s != null)
                        {
                            this.model.TranslateStartPositionTo(s.Index, virtualLocation.X, virtualLocation.Y);
                            this.view.SelectedItem = this.startPositionMapping[s.Index];
                        }
                    }
                }
            }
            finally
            {
                this.lastMousePos = virtualLocation;
            }
        }

        private void ViewMouseUp(object sender, MouseEventArgs e)
        {
            this.model.FlushTranslation();

            this.mouseDown = false;
        }

        #endregion

        private class FeatureTag
        {
            public Point Coordinates { get; set; }
        }

        private class StartPositionTag
        {
            public int Index { get; set; }
        }
    }
}
