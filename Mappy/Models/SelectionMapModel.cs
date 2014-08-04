namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Data;

    public class SelectionMapModel : ISelectionModel
    {
        private readonly IBindingMapModel model;

        private readonly ObservableCollection<GridCoordinates> selectedFeatures = new ObservableCollection<GridCoordinates>();

        private int? selectedTile;

        private int? selectedStartPosition;

        public SelectionMapModel(IBindingMapModel model)
        {
            this.model = model;

            model.FloatingTiles.ListChanged += this.FloatingTilesListChanged;

            model.Features.EntriesChanged += this.FeaturesEntriesChanged;
        }

        public event EventHandler SelectedTileChanged;

        public event EventHandler SelectedStartPositionChanged;

        public event EventHandler MinimapChanged
        {
            add
            {
                this.model.MinimapChanged += value;
            }

            remove
            {
                this.model.MinimapChanged -= value;
            }
        }

        public event EventHandler SeaLevelChanged
        {
            add
            {
                this.model.SeaLevelChanged += value;
            }

            remove
            {
                this.model.SeaLevelChanged -= value;
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

        public BindingSparseGrid<Feature> Features
        {
            get
            {
                return this.model.Features;
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

        IList<Positioned<IMapTile>> IMapModel.FloatingTiles
        {
            get
            {
                return this.FloatingTiles;
            }
        }

        ISparseGrid<Feature> IMapModel.Features
        {
            get
            {
                return this.Features;
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

        public ObservableCollection<GridCoordinates> SelectedFeatures
        {
            get
            {
                return this.selectedFeatures;
            }
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

        public void SelectFeature(GridCoordinates index)
        {
            this.SelectedFeatures.Add(index);
        }

        public void DeselectFeature(GridCoordinates index)
        {
            this.SelectedFeatures.Remove(index);
        }

        public void DeselectFeatures()
        {
            this.SelectedFeatures.Clear();
        }

        public void TranslateSelectedFeatures(int x, int y)
        {
            this.TranslateFeatures(this.SelectedFeatures, x, y);
        }

        public bool CanTranslateSelectedFeatures(int x, int y)
        {
            return this.CanTranslateFeatures(this.SelectedFeatures, x, y);
        }

        public void DeletedSelectedFeatures()
        {
            // take a copy, since the selected list will change
            // during deletion.
            var features = new List<GridCoordinates>(this.SelectedFeatures);

            foreach (var f in features)
            {
                this.model.Features.Remove(f.X, f.Y);
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

        private bool CanTranslateFeatures(IEnumerable<GridCoordinates> coords, int x, int y)
        {
            var coordSet = new HashSet<GridCoordinates>(coords);

            // pre-move check to see if anything is in our way
            foreach (var item in coordSet)
            {
                var translatedPoint = new GridCoordinates(item.X + x, item.Y + y);

                if (translatedPoint.X < 0
                    || translatedPoint.Y < 0
                    || translatedPoint.X >= this.model.Features.Width
                    || translatedPoint.Y >= this.model.Features.Height)
                {
                    return false;
                }

                bool isBlocked = !coordSet.Contains(translatedPoint)
                    && this.model.Features.HasValue(translatedPoint.X, translatedPoint.Y);
                if (isBlocked)
                {
                    return false;
                }
            }

            return true;
        }

        private void TranslateFeatures(IEnumerable<GridCoordinates> coords, int x, int y)
        {
            if (!this.CanTranslateFeatures(coords, x, y))
            {
                throw new ArgumentException("Specified translation would overwrite other features.");
            }

            var mapping = new Dictionary<GridCoordinates, Feature>();
            foreach (var i in coords)
            {
                mapping[i] = this.model.Features[i.X, i.Y];
                this.model.Features.Remove(i.X, i.Y);
            }

            foreach (var i in coords)
            {
                this.model.Features[i.X + x, i.Y + y] = mapping[i];
            }
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

        private void FeaturesEntriesChanged(object sender, SparseGridEventArgs e)
        {
            // keep the selected feature positions in sync
            switch (e.Action)
            {
                case SparseGridEventArgs.ActionType.Remove:
                    foreach (var f in e.Indexes)
                    {
                        // feature removed, remove it from selection
                        this.SelectedFeatures.Remove(this.model.Features.ToCoords(f));
                    }

                    break;

                case SparseGridEventArgs.ActionType.Move:
                    // feature moved, update the selected coords
                    // to match the feature's new coords
                    var oldIt = e.OriginalIndexes.GetEnumerator();
                    var newIt = e.Indexes.GetEnumerator();

                    // BUG: If there was a feature at the destination of another,
                    //      which was also moved, it might get overwritten.
                    //      This event only gets fired for single moves anyway,
                    //      so this should never happen.
                    while (oldIt.MoveNext() && newIt.MoveNext())
                    {
                        var oldCoord = this.Features.ToCoords(oldIt.Current);
                        var newCoord = this.Features.ToCoords(newIt.Current);

                        if (this.SelectedFeatures.Contains(oldCoord))
                        {
                            this.SelectedFeatures.Remove(oldCoord);
                            this.SelectedFeatures.Add(newCoord);
                        }
                    }

                    break;
            }
        }
    }
}
