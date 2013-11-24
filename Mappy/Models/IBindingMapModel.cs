namespace Mappy.Models
{
    using System;
    using System.ComponentModel;
    using Data;
    using Grids;

    public interface IBindingMapModel : IMapModel
    {
        event EventHandler MinimapChanged;

        event EventHandler SeaLevelChanged;

        new IBindingMapTile Tile { get; }

        new BindingList<Positioned<IMapTile>> FloatingTiles { get; }

        new IBindingSparseGrid<Feature> Features { get; }

        new IBindingSparseGrid<bool> Voids { get; }
    }
}
