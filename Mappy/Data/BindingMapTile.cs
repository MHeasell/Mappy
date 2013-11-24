namespace Mappy.Data
{
    using System.Drawing;
    using Grids;

    public class BindingMapTile : IBindingMapTile
    {
        private IMapTile baseTile;

        public BindingMapTile(IMapTile tile)
        {
            this.baseTile = tile;

            this.TileGrid = new BindingGrid<Bitmap>(tile.TileGrid);
            this.HeightGrid = new BindingGrid<int>(tile.HeightGrid);
        }

        public IBindingGrid<Bitmap> TileGrid { get; private set; }

        public IBindingGrid<int> HeightGrid { get; private set; }

        IGrid<Bitmap> IMapTile.TileGrid
        {
            get { return this.TileGrid; }
        }

        IGrid<int> IMapTile.HeightGrid
        {
            get { return this.HeightGrid; }
        }

        public void Merge(IMapTile other, int x, int y)
        {
            this.Merge(other, 0, 0, x, y, other.TileGrid.Width, other.TileGrid.Height);
        }

        public void Merge(IMapTile other, int sourceX, int sourceY, int destX, int destY, int width, int height)
        {
            this.TileGrid.Merge(other.TileGrid, sourceX, sourceY, destX, destY, width, height);
            this.HeightGrid.Merge(other.HeightGrid, sourceX * 2, sourceY * 2, destX * 2, destY * 2, width * 2, height * 2);
        }
    }
}
