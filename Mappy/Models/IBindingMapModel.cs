namespace Mappy.Models
{
    using System;
    using System.ComponentModel;
    using Data;

    using Mappy.Collections;

    public interface IBindingMapModel : IMapModel
    {
        event EventHandler MinimapChanged;

        event EventHandler SeaLevelChanged;

        new IBindingMapTile Tile { get; }

        new BindingList<Positioned<IMapTile>> FloatingTiles { get; }

        new BindingSparseGrid<Feature> Features { get; }

        new BindingSparseGrid<bool> Voids { get; }
    }
}
