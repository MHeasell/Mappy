namespace Mappy.Data
{
    using System;
    using System.Drawing;

    using Mappy.Collections;

    public class BindingMapTile : IMapTile
    {
        private readonly BindingGrid<Bitmap> tileGrid;

        private readonly BindingGrid<int> heightGrid;

        public BindingMapTile(IMapTile tile)
        {
            this.tileGrid = new BindingGrid<Bitmap>(tile.TileGrid);
            this.heightGrid = new BindingGrid<int>(tile.HeightGrid);

            this.tileGrid.CellsChanged += this.TileGridChanged;

            this.heightGrid.CellsChanged += this.HeightGridChanged;
        }

        public event EventHandler<GridEventArgs> TileGridChanged;

        public event EventHandler<GridEventArgs> HeightGridChanged;

        public IGrid<Bitmap> TileGrid
        {
            get { return this.tileGrid; }
        }

        public IGrid<int> HeightGrid
        {
            get { return this.heightGrid; }
        }
    }
}
