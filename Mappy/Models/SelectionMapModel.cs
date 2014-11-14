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

    public class SelectionMapModel : Notifier, ISelectionModel
    {
        private readonly IBindingMapModel model;

        private readonly ObservableCollection<Guid> selectedFeatures = new ObservableCollection<Guid>();

        private int? selectedTile;

        private int? selectedStartPosition;

        public SelectionMapModel(IBindingMapModel model)
        {
            this.model = model;

            model.PropertyChanged += this.ModelPropertyChanged;

            model.FloatingTiles.ListChanged += this.FloatingTilesListChanged;

            model.FeatureInstanceChanged += this.OnFeatureInstanceChanged;
        }

        public event EventHandler SelectedTileChanged;

        public event EventHandler SelectedStartPositionChanged;

        public event EventHandler<FeatureInstanceEventArgs> FeatureInstanceChanged
        {
            add
            {
                this.model.FeatureInstanceChanged += value;
            }

            remove
            {
                this.model.FeatureInstanceChanged -= value;
            }
        }

        public Bitmap Minimap
        {
            get
            {
                return this.model.Minimap;
            }

            set
            {
                this.model.Minimap = value;
            }
        }

        public int SeaLevel
        {
            get
            {
                return this.model.SeaLevel;
            }

            set
            {
                this.model.SeaLevel = value;
            }
        }

        public MapAttributes Attributes
        {
            get
            {
                return this.model.Attributes;
            }
        }

        public BindingMapTile Tile
        {
            get
            {
                return this.model.Tile;
            }
        }

        public BindingList<Positioned<IMapTile>> FloatingTiles
        {
            get
            {
                return this.model.FloatingTiles;
            }
        }

        public BindingSparseGrid<bool> Voids
        {
            get
            {
                return this.model.Voids;
            }
        }

        IMapTile IMapModel.Tile
        {
            get
            {
                return this.Tile;
            }
        }

        public int FeatureGridWidth
        {
            get
            {
                return this.model.FeatureGridWidth;
            }
        }

        public int FeatureGridHeight
        {
            get
            {
                return this.model.FeatureGridHeight;
            }
        }

        IList<Positioned<IMapTile>> IMapModel.FloatingTiles
        {
            get
            {
                return this.FloatingTiles;
            }
        }

        ISparseGrid<bool> IMapModel.Voids
        {
            get
            {
                return this.Voids;
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
                if (this.selectedTile != value)
                {
                    this.selectedTile = value;
                    var h = this.SelectedTileChanged;
                    if (h != null)
                    {
                        h(this, EventArgs.Empty);
                    }
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
                if (this.selectedStartPosition != value)
                {
                    this.selectedStartPosition = value;
                    var h = this.SelectedStartPositionChanged;
                    if (h != null)
                    {
                        h(this, EventArgs.Empty);
                    }
                }
            }
        }

        public ObservableCollection<Guid> SelectedFeatures
        {
            get
            {
                return this.selectedFeatures;
            }
        }

        public void AddFeatureInstance(FeatureInstance instance)
        {
            this.model.AddFeatureInstance(instance);
        }

        public FeatureInstance GetFeatureInstance(Guid id)
        {
            return this.model.GetFeatureInstance(id);
        }

        public FeatureInstance GetFeatureInstanceAt(int x, int y)
        {
            return this.model.GetFeatureInstanceAt(x, y);
        }

        public void RemoveFeatureInstance(Guid id)
        {
            this.model.RemoveFeatureInstance(id);
        }

        public bool HasFeatureInstanceAt(int x, int y)
        {
            return this.model.HasFeatureInstanceAt(x, y);
        }

        public void UpdateFeatureInstance(FeatureInstance instance)
        {
            this.model.UpdateFeatureInstance(instance);
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

            var tile = this.model.FloatingTiles[this.SelectedTile.Value];
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

            this.model.FloatingTiles.RemoveAt(this.SelectedTile.Value);
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
                this.model.RemoveFeatureInstance(f);
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

            var pos = this.model.Attributes.GetStartPosition(this.SelectedStartPosition.Value);

            if (!pos.HasValue)
            {
                throw new InvalidOperationException("Selected start position has not been placed.");
            }

            var newPos = pos.Value;
            newPos.X += x;
            newPos.Y += y;
            this.model.Attributes.SetStartPosition(this.SelectedStartPosition.Value, newPos);
        }

        public void DeleteSelectedStartPosition()
        {
            if (!this.SelectedStartPosition.HasValue)
            {
                throw new InvalidOperationException("No start position selected.");
            }

            this.model.Attributes.SetStartPosition(this.SelectedStartPosition.Value, null);
        }

        public void DeselectAll()
        {
            this.DeselectFeatures();
            this.DeselectStartPosition();
            this.DeselectTile();
        }

        public IEnumerable<FeatureInstance> EnumerateFeatureInstances()
        {
            return this.model.EnumerateFeatureInstances();
        }

        private void MergeTile(int index)
        {
            var tile = this.model.FloatingTiles[index];
            var src = tile.Item;
            var x = tile.Location.X;
            var y = tile.Location.Y;

            var dst = this.model.Tile;

            // construct the destination target
            Rectangle rect = new Rectangle(x, y, src.TileGrid.Width, src.TileGrid.Height);

            // clip to boundaries
            rect.Intersect(new Rectangle(0, 0, dst.TileGrid.Width, dst.TileGrid.Height));

            int srcX = rect.X - x;
            int srcY = rect.Y - y;

            GridMethods.Copy(src.TileGrid, dst.TileGrid, srcX, srcY, rect.X, rect.Y, rect.Width, rect.Height);
            GridMethods.Copy(src.HeightGrid, dst.HeightGrid, srcX * 2, srcY * 2, rect.X * 2, rect.Y * 2, rect.Width * 2, rect.Height * 2);
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

        private void OnFeatureInstanceChanged(object sender, FeatureInstanceEventArgs e)
        {
            switch (e.Action)
            {
                case FeatureInstanceEventArgs.ActionType.Remove:
                    this.SelectedFeatures.Remove(e.FeatureInstanceId);
                    break;
            }
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Minimap":
                case "SeaLevel":
                    this.FireChange(e.PropertyName);
                    break;
            }
        }
    }
}
