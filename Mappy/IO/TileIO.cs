namespace Mappy.IO
{
    using System.Drawing;
    using Mappy.Data;
    using TAUtil.Sct;

    public static class TileIO
    {
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
    }
}