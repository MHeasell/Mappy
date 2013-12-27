namespace Mappy.Models
{
    using System.Collections.Generic;
    using System.Drawing;
    using Data;

    using Mappy.Collections;

    public interface IMapModel
    {
        IMapTile Tile { get; }

        IList<Positioned<IMapTile>> FloatingTiles { get; }

        ISparseGrid<Feature> Features { get; }

        ISparseGrid<bool> Voids { get; }

        Bitmap Minimap { get; set; }

        int SeaLevel { get; set; }

        MapAttributes Attributes { get; }
    }
}
