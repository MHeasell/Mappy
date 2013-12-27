namespace Mappy.Data
{
    using System.Drawing;

    using Mappy.Collections;

    public interface IBindingMapTile : IMapTile
    {
        new IBindingGrid<Bitmap> TileGrid { get; }

        new IBindingGrid<int> HeightGrid { get; }
    }
}
