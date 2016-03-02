namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.Util;

    public class MapModel : Notifier, ISelectionModel
    {
        private readonly SparseGrid<FeatureInstance> featureLocationIndex;

        private readonly Dictionary<Guid, FeatureInstance> featureInstances = new Dictionary<Guid, FeatureInstance>();

        private readonly BindingMapTile tile;

        private readonly BindingList<Positioned<IMapTile>> floatingTiles;

        private readonly BindingSparseGrid<bool> voids;

        private readonly ObservableCollection<Guid> selectedFeatures = new ObservableCollection<Guid>();

        private Bitmap minimap;

        private int seaLevel;

        private int? selectedStartPosition;

        private int? selectedTile;

        public MapModel(int width, int height)
            : this(width, height, new MapAttributes())
        {
        }

        public MapModel(int width, int height, MapAttributes attrs)
            : this(new MapTile(width, height), attrs)
        {
        }

        public MapModel(MapTile tile)
            : this(tile, new MapAttributes())
        {
        }

        public MapModel(MapTile tile, MapAttributes attrs)
        {
            this.tile = new BindingMapTile(tile);

            this.Attributes = attrs;

            this.floatingTiles = new BindingList<Positioned<IMapTile>>();

            this.voids = new BindingSparseGrid<bool>(new SparseGrid<bool>(this.Tile.HeightGrid.Width, this.Tile.HeightGrid.Height));

            this.featureLocationIndex = new SparseGrid<FeatureInstance>(this.Tile.HeightGrid.Width, this.Tile.HeightGrid.Height);

            this.Minimap = new Bitmap(252, 252);
            var g = Graphics.FromImage(this.Minimap);
            g.FillRectangle(Brushes.White, 0, 0, this.Minimap.Width, this.Minimap.Height);

            this.FloatingTilesChanged += this.FloatingTilesListChanged;
        }

        public event EventHandler<FeatureInstanceEventArgs> FeatureInstanceChanged;

        public event EventHandler<GridEventArgs> TileGridChanged
        {
            add
            {
                this.tile.TileGridChanged += value;
            }

            remove
            {
                this.tile.TileGridChanged -= value;
            }
        }

        public event EventHandler<GridEventArgs> HeightGridChanged
        {
            add
            {
                this.tile.HeightGridChanged += value;
            }

            remove
            {
                this.tile.HeightGridChanged -= value;
            }
        }

        public event ListChangedEventHandler FloatingTilesChanged
        {
            add
            {
                this.floatingTiles.ListChanged += value;
            }

            remove
            {
                this.floatingTiles.ListChanged -= value;
            }
        }

        public event EventHandler<SparseGridEventArgs> VoidsChanged
        {
            add
            {
                this.voids.EntriesChanged += value;
            }

            remove
            {
                this.voids.EntriesChanged -= value;
            }
        }

        public MapAttributes Attributes { get; }

        public IMapTile Tile => this.tile;

        public IList<Positioned<IMapTile>> FloatingTiles => this.floatingTiles;

        public ISparseGrid<bool> Voids => this.voids;

        public int SeaLevel
        {
            get
            {
                return this.seaLevel;
            }

            set
            {
                this.SetField(ref this.seaLevel, value, "SeaLevel");
            }
        }

        public Bitmap Minimap
        {
            get
            {
                return this.minimap;
            }

            set
            {
                this.SetField(ref this.minimap, value, "Minimap");
            }
        }

        public int FeatureGridWidth => this.Tile.HeightGrid.Width;

        public int FeatureGridHeight => this.Tile.HeightGrid.Height;

        public int? SelectedStartPosition
        {
            get
            {
                return this.selectedStartPosition;
            }

            private set
            {
                this.SetField(ref this.selectedStartPosition, value, "SelectedStartPosition");
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
                this.SetField(ref this.selectedTile, value, "SelectedTile");
            }
        }

        public ObservableCollection<Guid> SelectedFeatures => this.selectedFeatures;

        /// <summary>
        /// <see cref="IMapModel.AddFeatureInstance"/>
        /// </summary>
        public void AddFeatureInstance(FeatureInstance instance)
        {
            this.AddFeatureInstanceInternal(instance);

            var arg = new FeatureInstanceEventArgs(
                FeatureInstanceEventArgs.ActionType.Add, instance.Id);
            this.OnFeatureInstanceChanged(arg);
        }

        /// <summary>
        /// <see cref="IReadOnlyMapModel.GetFeatureInstance"/>
        /// </summary>
        public FeatureInstance GetFeatureInstance(Guid id)
        {
            return this.featureInstances[id];
        }

        /// <summary>
        /// <see cref="IReadOnlyMapModel.GetFeatureInstanceAt"/>
        /// </summary>
        public FeatureInstance GetFeatureInstanceAt(int x, int y)
        {
            FeatureInstance inst;
            if (!this.featureLocationIndex.TryGetValue(x, y, out inst))
            {
                return null;
            }

            return inst;
        }

        /// <summary>
        /// <see cref="IMapModel.UpdateFeatureInstance"/>
        /// </summary>
        public void UpdateFeatureInstance(FeatureInstance instance)
        {
            if (!this.featureInstances.ContainsKey(instance.Id))
            {
                throw new ArgumentException("No existing FeatureInstance with this ID.");
            }

            this.RemoveFeatureInstanceInternal(instance.Id);
            this.AddFeatureInstanceInternal(instance);

            var arg = new FeatureInstanceEventArgs(
                FeatureInstanceEventArgs.ActionType.Move,
                instance.Id);
            this.OnFeatureInstanceChanged(arg);
        }

        /// <summary>
        /// <see cref="IMapModel.RemoveFeatureInstance"/>
        /// </summary>
        public void RemoveFeatureInstance(Guid id)
        {
            this.RemoveFeatureInstanceInternal(id);

            var arg = new FeatureInstanceEventArgs(
                FeatureInstanceEventArgs.ActionType.Remove,
                id);
            this.OnFeatureInstanceChanged(arg);
        }

        /// <summary>
        /// <see cref="IReadOnlyMapModel.HasFeatureInstanceAt"/>
        /// </summary>
        public bool HasFeatureInstanceAt(int x, int y)
        {
            return this.featureLocationIndex.HasValue(x, y);
        }

        /// <summary>
        /// <see cref="IReadOnlyMapModel.EnumerateFeatureInstances"/>
        /// </summary>
        public IEnumerable<FeatureInstance> EnumerateFeatureInstances()
        {
            return this.featureInstances.Values;
        }

        public void SelectTile(int index)
        {
            this.SelectedTile = index;
        }

        public void DeselectTile()
        {
            this.SelectedTile = null;
        }

        public void TranslateSelectedTile(int x, int y)
        {
            if (!this.SelectedTile.HasValue)
            {
                throw new InvalidOperationException("No tile selected.");
            }

            var tile = this.FloatingTiles[this.SelectedTile.Value];
            var loc = tile.Location;
            loc.X += x;
            loc.Y += y;
            tile.Location = loc;
        }

        public void DeleteSelectedTile()
        {
            if (!this.SelectedTile.HasValue)
            {
                throw new InvalidOperationException("No tile selected.");
            }

            this.FloatingTiles.RemoveAt(this.SelectedTile.Value);
        }

        public void MergeSelectedTile()
        {
            if (!this.SelectedTile.HasValue)
            {
                throw new InvalidOperationException("No tile selected.");
            }

            this.MergeTile(this.SelectedTile.Value);
        }

        public void SelectFeature(Guid id)
        {
            this.SelectedFeatures.Add(id);
        }

        public void DeselectFeature(Guid id)
        {
            this.SelectedFeatures.Remove(id);
        }

        public void DeselectFeatures()
        {
            this.SelectedFeatures.Clear();
        }

        public void DeletedSelectedFeatures()
        {
            // take a copy, since the selected list will change
            // during deletion.
            var features = new List<Guid>(this.SelectedFeatures);

            foreach (var f in features)
            {
                this.RemoveFeatureInstance(f);
            }
        }

        public void SelectStartPosition(int index)
        {
            this.SelectedStartPosition = index;
        }

        public void DeselectStartPosition()
        {
            this.SelectedStartPosition = null;
        }

        public void TranslateSelectedStartPosition(int x, int y)
        {
            if (!this.SelectedStartPosition.HasValue)
            {
                throw new InvalidOperationException("No start position selected.");
            }

            var pos = this.Attributes.GetStartPosition(this.SelectedStartPosition.Value);

            if (!pos.HasValue)
            {
                throw new InvalidOperationException("Selected start position has not been placed.");
            }

            var newPos = pos.Value;
            newPos.X += x;
            newPos.Y += y;
            this.Attributes.SetStartPosition(this.SelectedStartPosition.Value, newPos);
        }

        public void DeleteSelectedStartPosition()
        {
            if (!this.SelectedStartPosition.HasValue)
            {
                throw new InvalidOperationException("No start position selected.");
            }

            this.Attributes.SetStartPosition(this.SelectedStartPosition.Value, null);
        }

        public void DeselectAll()
        {
            this.DeselectFeatures();
            this.DeselectStartPosition();
            this.DeselectTile();
        }

        private void AddFeatureInstanceInternal(FeatureInstance instance)
        {
            if (this.featureInstances.ContainsKey(instance.Id))
            {
                throw new ArgumentException("A FeatureInstance with the given ID already exists.");
            }

            if (this.featureLocationIndex.HasValue(instance.X, instance.Y))
            {
                throw new ArgumentException("A FeatureInstance is already present at the target location.");
            }

            this.featureInstances[instance.Id] = instance;
            this.featureLocationIndex.Set(instance.X, instance.Y, instance);
        }

        private void RemoveFeatureInstanceInternal(Guid id)
        {
            if (!this.featureInstances.ContainsKey(id))
            {
                throw new ArgumentException("No existing FeatureInstance with this ID.");
            }

            var inst = this.GetFeatureInstance(id);
            this.featureInstances.Remove(id);
            this.featureLocationIndex.Remove(inst.X, inst.Y);
        }

        private void FloatingTilesListChanged(object sender, ListChangedEventArgs e)
        {
            if (!this.SelectedTile.HasValue)
            {
                return;
            }

            // keep the index of the selected tile in sync with the list
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (e.NewIndex <= this.SelectedTile.Value)
                    {
                        // item was added before us, so bump our index
                        this.SelectedTile++;
                    }

                    break;

                case ListChangedType.ItemDeleted:
                    if (e.NewIndex == this.SelectedTile.Value)
                    {
                        // we were deleted, remove selection
                        this.SelectedTile = null;
                    }
                    else if (e.NewIndex < this.SelectedTile.Value)
                    {
                        // item was deleted before us, so decrement our index
                        this.SelectedTile--;
                    }

                    break;

                case ListChangedType.ItemMoved:
                    if (e.OldIndex == this.SelectedTile.Value)
                    {
                        // we were moved, update to new index
                        this.SelectedTile = e.NewIndex;
                    }
                    else
                    {
                        if (e.OldIndex < this.SelectedTile.Value)
                        {
                            // item was removed before us, decrement our index
                            this.SelectedTile--;
                        }

                        if (e.NewIndex <= this.SelectedTile.Value)
                        {
                            // item was inserted before us, bump our index
                            this.SelectedTile++;
                        }
                    }

                    break;
            }
        }

        private void OnFeatureInstanceChanged(FeatureInstanceEventArgs e)
        {
            var h = this.FeatureInstanceChanged;
            if (h != null)
            {
                h(this, e);
            }

            switch (e.Action)
            {
                case FeatureInstanceEventArgs.ActionType.Remove:
                    this.SelectedFeatures.Remove(e.FeatureInstanceId);
                    break;
            }
        }

        private void MergeTile(int index)
        {
            var tile = this.FloatingTiles[index];
            var src = tile.Item;
            var x = tile.Location.X;
            var y = tile.Location.Y;

            var dst = this.Tile;

            // construct the destination target
            Rectangle rect = new Rectangle(x, y, src.TileGrid.Width, src.TileGrid.Height);

            // clip to boundaries
            rect.Intersect(new Rectangle(0, 0, dst.TileGrid.Width, dst.TileGrid.Height));

            int srcX = rect.X - x;
            int srcY = rect.Y - y;

            GridMethods.Copy(src.TileGrid, dst.TileGrid, srcX, srcY, rect.X, rect.Y, rect.Width, rect.Height);
            GridMethods.Copy(src.HeightGrid, dst.HeightGrid, srcX * 2, srcY * 2, rect.X * 2, rect.Y * 2, rect.Width * 2, rect.Height * 2);
        }
    }
}
