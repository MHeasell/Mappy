namespace Mappy.Data
{
    using System.Diagnostics;
    using System.Drawing;
    using Grids;

    using TAUtil.Sct;

    public class MapTile : IMapTile
    {
        public const int TileWidth = 32;
        public const int TileHeight = 32;
        public const int AttrWidth = TileWidth / 2;
        public const int AttrHeight = TileHeight / 2;

        public static readonly Size TileSize = new Size(MapTile.TileWidth, MapTile.TileHeight);
        public static readonly Size AttrSize = new Size(MapTile.AttrWidth, MapTile.AttrHeight);

        private static readonly Bitmap DefaultSquare = new Bitmap(MapTile.TileWidth, MapTile.TileHeight);

        public MapTile(int width, int height)
        {
            this.TileGrid = new Grid<Bitmap>(width, height);
            this.HeightGrid = new Grid<int>(width * 2, height * 2);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.TileGrid.Set(x, y, MapTile.DefaultSquare);
                }
            }
        }

        public MapTile(Bitmap[,] graphics, int[,] heightField)
        {
            Debug.Assert(graphics.GetLength(0) * 2 == heightField.GetLength(0), "sizes do not match");
            Debug.Assert(graphics.GetLength(1) * 2 == heightField.GetLength(1), "sizes do not match");

            this.TileGrid = Grid<Bitmap>.GetView(graphics);
            this.HeightGrid = Grid<int>.GetView(heightField);
        }

        public IGrid<Bitmap> TileGrid { get; private set; }

        public IGrid<int> HeightGrid { get; private set; }

        public static MapTile ReadFromSct(SctReader f, Color[] palette)
        {
            MapTile tile = new MapTile(f.Width, f.Height);

            {
                int i = 0;
                foreach (var t in f.EnumerateTileDataBitmaps(palette))
                {
                    tile.TileGrid.Set(i % f.Width, i / f.Width, t);
                    i++;
                }
            }

            {
                int j = 0;
                foreach (var attr in f.Attrs)
                {
                    tile.HeightGrid.Set(j % (f.Width * 2), j / (f.Width * 2), attr.Height);
                    j++;
                }
            }

            return tile;
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
