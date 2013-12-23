namespace Mappy.IO
{
    using System.Drawing;

    using Mappy.Data;
    using Mappy.Util;

    using TAUtil.Sct;

    public static class TileIO
    {
        public static MapTile ReadFromSct(SctReader f, Color[] palette)
        {
            MapTile tile = new MapTile(f.Width, f.Height);

            f.SeekToTiles();
            Bitmap[] tiles = ReadTiles(f, palette);

            f.SeekToData();
            for (int y = 0; y < f.Height; y++)
            {
                for (int x = 0; x < f.Width; x++)
                {
                    tile.TileGrid.Set(x, y, tiles[f.ReadDataCell()]);
                }
            }

            f.SeekToAttrs();
            for (int y = 0; y < f.HeightInAttrs; y++)
            {
                for (int x = 0; x < f.WidthInAttrs; x++)
                {
                    tile.HeightGrid.Set(x, y, f.ReadAttr().Height);
                }
            }

            return tile;
        }

        private static Bitmap[] ReadTiles(SctReader reader, Color[] palette)
        {
            Bitmap[] bitmaps = new Bitmap[reader.TileCount];

            for (int i = 0; i < reader.TileCount; i++)
            {
                bitmaps[i] = Util.AddTileToDatabase(reader.ReadTile(), palette);
            }

            return bitmaps;
        }
    }
}