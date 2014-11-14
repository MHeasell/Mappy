namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using Data;

    using Mappy.Collections;
    using Mappy.Util;

    public class BindingMapModel : Notifier, IBindingMapModel
    {
        private readonly IMapModel model;

        public BindingMapModel(IMapModel model)
        {
            this.model = model;

            this.Tile = new BindingMapTile(model.Tile);
            this.FloatingTiles = new BindingList<Positioned<IMapTile>>(model.FloatingTiles);
            this.Voids = new BindingSparseGrid<bool>(model.Voids);
        }

        public event EventHandler<FeatureInstanceEventArgs> FeatureInstanceChanged;

        public BindingMapTile Tile
        {
            get;
            private set;
        }

        public BindingSparseGrid<bool> Voids
        {
            get;
            private set;
        }

        public BindingList<Positioned<IMapTile>> FloatingTiles
        {
            get;
            private set;
        }

        public Bitmap Minimap
        {
            get
            {
                return this.model.Minimap;
            }

            set
            {
                if (this.model.Minimap != value)
                {
                    this.model.Minimap = value;
                    this.FireChange("Minimap");
                }
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
                if (this.model.SeaLevel != value)
                {
                    this.model.SeaLevel = value;
                    this.FireChange("SeaLevel");
                }
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

        public MapAttributes Attributes
        {
            get { return this.model.Attributes; }
        }

        IMapTile IMapModel.Tile
        {
            get { return this.Tile; }
        }

        ISparseGrid<bool> IMapModel.Voids
        {
            get { return this.Voids; }
        }

        IList<Positioned<IMapTile>> IMapModel.FloatingTiles
        {
            get { return this.FloatingTiles; }
        }

        public FeatureInstance GetFeatureInstance(Guid id)
        {
            return this.model.GetFeatureInstance(id);
        }

        public FeatureInstance GetFeatureInstanceAt(int x, int y)
        {
            return this.model.GetFeatureInstanceAt(x, y);
        }

        public void AddFeatureInstance(FeatureInstance instance)
        {
            this.model.AddFeatureInstance(instance);
            var arg = new FeatureInstanceEventArgs(
                FeatureInstanceEventArgs.ActionType.Add,
                instance.Id);
            this.OnFeatureInstanceChanged(arg);
        }

        public void RemoveFeatureInstance(Guid id)
        {
            this.model.RemoveFeatureInstance(id);
            var arg = new FeatureInstanceEventArgs(
                FeatureInstanceEventArgs.ActionType.Remove,
                id);
            this.OnFeatureInstanceChanged(arg);
        }

        public void UpdateFeatureInstance(FeatureInstance instance)
        {
            this.model.UpdateFeatureInstance(instance);
            var arg = new FeatureInstanceEventArgs(
                FeatureInstanceEventArgs.ActionType.Move,
                instance.Id);
            this.OnFeatureInstanceChanged(arg);
        }

        public bool HasFeatureInstanceAt(int x, int y)
        {
            return this.model.HasFeatureInstanceAt(x, y);
        }

        public IEnumerable<FeatureInstance> EnumerateFeatureInstances()
        {
            return this.model.EnumerateFeatureInstances();
        }

        protected virtual void OnFeatureInstanceChanged(FeatureInstanceEventArgs e)
        {
            var h = this.FeatureInstanceChanged;
            if (h != null)
            {
                h(this, e);
            }
        }
    }
}
