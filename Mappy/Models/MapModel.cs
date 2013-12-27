namespace Mappy.Models
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Data;

    using Mappy.Collections;

    public class MapModel : IMapModel
    {
        public MapModel(int width, int height)
            : this(width, height, new MapAttributes())
        {
        }

        public MapModel(int width, int height, MapAttributes attrs)
            : this(new MapTile(width, height), attrs)
        {
        }

        public MapModel(MapTile tile)
            : this(tile, new MapAttributes())
        {
        }

        public MapModel(MapTile tile, MapAttributes attrs)
        {
            this.Tile = tile;
            this.Attributes = attrs;
            this.FloatingTiles = new List<Positioned<IMapTile>>();
            this.Features = new SparseGrid<Feature>(this.Tile.HeightGrid.Width, this.Tile.HeightGrid.Height);
            this.Voids = new SparseGrid<bool>(this.Tile.HeightGrid.Width, this.Tile.HeightGrid.Height);
            this.Minimap = new Bitmap(252, 252);
        }

        public MapAttributes Attributes { get; private set; }

        public IMapTile Tile { get; private set; }

        public IList<Positioned<IMapTile>> FloatingTiles { get; private set; }

        public ISparseGrid<Feature> Features
        {
            get;
            private set;
        }

        public ISparseGrid<bool> Voids
        {
            get;
            private set;
        }

        public int SeaLevel { get; set; }

        public Bitmap Minimap { get; set; }

        public Bitmap GenerateMinimap()
        {
            int mapWidth = this.Tile.TileGrid.Width * 32;
            int mapHeight = this.Tile.TileGrid.Height * 32;

            int width, height;

            if (this.Tile.TileGrid.Width > this.Tile.TileGrid.Height)
            {
                width = 252;
                height = (int)(252 * (mapHeight / (float)mapWidth));
            }
            else
            {
                height = 252;
                width = (int)(252 * (mapWidth / (float)mapHeight));
            }

            Bitmap b = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int imageX = (int)((x / (float)width) * mapWidth);
                    int imageY = (int)((y / (float)height) * mapHeight);
                    b.SetPixel(x, y, this.GetPixel(imageX, imageY));
                }
            }

            return b;
        }

        private Color GetPixel(int x, int y)
        {
            int tileX = x / 32;
            int tileY = y / 32;

            int tilePixelX = x % 32;
            int tilePixelY = y % 32;

            foreach (Positioned<IMapTile> t in this.FloatingTiles.Reverse())
            {
                Rectangle r = new Rectangle(t.Location, new Size(t.Item.TileGrid.Width, t.Item.TileGrid.Height));
                if (r.Contains(tileX, tileY))
                {
                    return t.Item.TileGrid.Get(tileX - t.Location.X, tileY - t.Location.Y).GetPixel(tilePixelX, tilePixelY);
                }
            }

            return this.Tile.TileGrid.Get(tileX, tileY).GetPixel(tilePixelX, tilePixelY);
        }
    }
}
