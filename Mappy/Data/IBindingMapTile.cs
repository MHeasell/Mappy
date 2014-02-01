namespace Mappy.Data
{
    using System.Drawing;

    using Mappy.Collections;

    public interface IBindingMapTile : IMapTile
    {
        new BindingGrid<Bitmap> TileGrid { get; }

        new BindingGrid<int> HeightGrid { get; }
    }
}
