namespace Mappy.Data
{
    using System.Drawing;
    using Grids;

    public interface IBindingMapTile : IMapTile
    {
        new IBindingGrid<Bitmap> TileGrid { get; }

        new IBindingGrid<int> HeightGrid { get; }
    }
}
