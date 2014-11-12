namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Data;

    using Mappy.Collections;

    public class MapModel : IMapModel
    {
        private readonly SparseGrid<Guid> featureLocationIndex;

        private readonly Dictionary<Guid, FeatureInstance> featureInstances = new Dictionary<Guid, FeatureInstance>();

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
            this.Tile = tile;
            this.Attributes = attrs;
            this.FloatingTiles = new List<Positioned<IMapTile>>();
            this.Voids = new SparseGrid<bool>(this.Tile.HeightGrid.Width, this.Tile.HeightGrid.Height);
            this.featureLocationIndex = new SparseGrid<Guid>(this.Tile.HeightGrid.Width, this.Tile.HeightGrid.Height);
            this.Minimap = new Bitmap(252, 252);
            var g = Graphics.FromImage(this.Minimap);
            g.FillRectangle(Brushes.White, 0, 0, this.Minimap.Width, this.Minimap.Height);
        }

        public MapAttributes Attributes { get; private set; }

        public IMapTile Tile { get; private set; }

        public IList<Positioned<IMapTile>> FloatingTiles { get; private set; }

        public ISparseGrid<bool> Voids
        {
            get;
            private set;
        }

        public int SeaLevel { get; set; }

        public Bitmap Minimap { get; set; }

        public int FeatureGridWidth
        {
            get
            {
                return this.Tile.HeightGrid.Width;
            }
        }

        public int FeatureGridHeight
        {
            get
            {
                return this.Tile.HeightGrid.Height;
            }
        }

        public void AddFeatureInstance(FeatureInstance instance)
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
            this.featureLocationIndex[instance.X, instance.Y] = instance.Id;
        }

        public FeatureInstance GetFeatureInstance(Guid id)
        {
            return this.featureInstances[id];
        }

        public FeatureInstance GetFeatureInstanceAt(int x, int y)
        {
            Guid id;
            if (!this.featureLocationIndex.TryGetValue(x, y, out id))
            {
                return null;
            }

            return this.GetFeatureInstance(id);
        }

        public void UpdateFeatureInstance(FeatureInstance instance)
        {
            if (!this.featureInstances.ContainsKey(instance.Id))
            {
                throw new ArgumentException("No existing FeatureInstance with this ID.");
            }

            this.RemoveFeatureInstance(instance.Id);
            this.AddFeatureInstance(instance);
        }

        public void RemoveFeatureInstance(Guid id)
        {
            if (!this.featureInstances.ContainsKey(id))
            {
                throw new ArgumentException("No existing FeatureInstance with this ID.");
            }

            var inst = this.GetFeatureInstance(id);
            this.featureInstances.Remove(id);
            this.featureLocationIndex.Remove(inst.X, inst.Y);
        }

        public bool HasFeatureInstanceAt(int x, int y)
        {
            return this.featureLocationIndex.HasValue(x, y);
        }

        public IEnumerable<FeatureInstance> EnumerateFeatureInstances()
        {
            return this.featureInstances.Values;
        }
    }
}
