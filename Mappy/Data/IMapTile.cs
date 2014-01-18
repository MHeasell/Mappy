namespace Mappy.Data
{
    using System.Drawing;

    using Mappy.Collections;

    public interface IMapTile
    {
        IGrid<Bitmap> TileGrid { get; }

        IGrid<int> HeightGrid { get; }
    }
}
