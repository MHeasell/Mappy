namespace Mappy.Data
{
    using System;
    using System.Drawing;

    using Mappy.Collections;

    /// <summary>
    /// Represents a tile on the map.
    /// </summary>
    [Serializable]
    public class MapTile : IMapTile
    {
        public const int TileWidth = 32;
        public const int TileHeight = 32;
        public const int AttrWidth = TileWidth / 2;
        public const int AttrHeight = TileHeight / 2;

        public static readonly Size TileSize = new Size(MapTile.TileWidth, MapTile.TileHeight);
        public static readonly Size AttrSize = new Size(MapTile.AttrWidth, MapTile.AttrHeight);

        public MapTile(int width, int height)
        {
            this.TileGrid = new Grid<Bitmap>(width, height);
            this.HeightGrid = new Grid<int>(width * 2, height * 2);
        }

        public IGrid<Bitmap> TileGrid { get; private set; }

        public IGrid<int> HeightGrid { get; private set; }
    }
}
