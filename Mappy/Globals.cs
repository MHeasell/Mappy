namespace Mappy
{
    using System.Drawing;
    using System.IO;

    using Mappy.IO;

    using TAUtil.Gdi.Palette;

    public static class Globals
    {
        public static readonly BitmapCache TileCache;

        public static readonly Bitmap DefaultTile;

        static Globals()
        {
            TileCache = new BitmapCache();

            DefaultTile = new Bitmap(32, 32);
            var g = Graphics.FromImage(DefaultTile);
            g.FillRectangle(Brushes.White, 0, 0, DefaultTile.Width, DefaultTile.Height);
        }
    }
}
