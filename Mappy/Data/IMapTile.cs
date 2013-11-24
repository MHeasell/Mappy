namespace Mappy.Data
{
    using System.Drawing;
    using Grids;

    public interface IMapTile
    {
        IGrid<Bitmap> TileGrid { get; }

        IGrid<int> HeightGrid { get; }

        void Merge(IMapTile other, int x, int y);

        void Merge(IMapTile other, int sourceX, int sourceY, int destX, int destY, int width, int height);
    }
}
