namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using Data;
    using Grids;

    public class BindingMapModel : IBindingMapModel
    {
        private readonly IMapModel model;

        public BindingMapModel(IMapModel model)
        {
            this.model = model;

            this.Tile = new BindingMapTile(model.Tile);
            this.FloatingTiles = new BindingList<Positioned<IMapTile>>(model.FloatingTiles);
            this.Features = new BindingSparseGrid<Feature>(model.Features);
            this.Voids = new BindingSparseGrid<bool>(model.Voids);
        }

        public event EventHandler MinimapChanged;

        public event EventHandler SeaLevelChanged;

        public IBindingMapTile Tile
        {
            get;
            private set;
        }

        public IBindingSparseGrid<Feature> Features
        {
            get;
            private set;
        }

        public IBindingSparseGrid<bool> Voids
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
                this.model.Minimap = value;
                this.OnMinimapChanged();
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
                this.OnSeaLevelChanged();
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

        ISparseGrid<Feature> IMapModel.Features
        {
            get { return this.Features; }
        }

        ISparseGrid<bool> IMapModel.Voids
        {
            get { return this.Voids; }
        }

        IList<Positioned<IMapTile>> IMapModel.FloatingTiles
        {
            get { return this.FloatingTiles; }
        }

        private void OnSeaLevelChanged()
        {
            EventHandler h = this.SeaLevelChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }

        private void OnMinimapChanged()
        {
            EventHandler h = this.MinimapChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }
    }
}
