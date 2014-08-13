namespace Mappy.Util.ImageSampling
{
    using System.Drawing;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Models;

    public class MapPixelImageAdapter : IPixelImage
    {
        private readonly IMapModel map;

        public MapPixelImageAdapter(IMapModel map)
        {
            this.map = map;
        }

        public int Width
        {
            get
            {
                return (this.map.Tile.TileGrid.Width * 32) - 32;
            }
        }

        public int Height
        {
            get
            {
                return (this.map.Tile.TileGrid.Height * 32) - 128;
            }
        }

        public Color this[int x, int y]
        {
            get
            {
                int tileX = x / 32;
                int tileY = y / 32;

                int tilePixelX = x % 32;
                int tilePixelY = y % 32;

                foreach (Positioned<IMapTile> t in this.map.FloatingTiles.Reverse())
                {
                    Rectangle r = new Rectangle(t.Location, new Size(t.Item.TileGrid.Width, t.Item.TileGrid.Height));
                    if (r.Contains(tileX, tileY))
                    {
                        return t.Item.TileGrid[tileX - t.Location.X, tileY - t.Location.Y].GetPixel(
                            tilePixelX,
                            tilePixelY);
                    }
                }

                Bitmap bitmap = this.map.Tile.TileGrid[tileX, tileY];
                if (bitmap == null)
                {
                    return Color.Black;
                }

                return bitmap.GetPixel(tilePixelX, tilePixelY);
            }
        }
    }
}
