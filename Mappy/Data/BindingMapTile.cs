namespace Mappy.Data
{
    using System.Drawing;

    using Mappy.Collections;

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
    }
}
