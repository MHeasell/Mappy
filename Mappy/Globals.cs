namespace Mappy
{
    using System.Drawing;
    using System.IO;

    using Mappy.IO;

    using TAUtil.Gdi.Palette;

    public static class Globals
    {
        public static readonly IPalette Palette;

        public static readonly BitmapCache TileCache;

        public static readonly Bitmap DefaultTile;

        static Globals()
        {
            using (var s = new MemoryStream(Mappy.Properties.Resources.PALETTE))
            {
                Palette = PaletteFactory.FromBinaryPal(s);
            }

            TileCache = new BitmapCache();

            DefaultTile = new Bitmap(32, 32);
            var g = Graphics.FromImage(DefaultTile);
            g.FillRectangle(Brushes.White, 0, 0, DefaultTile.Width, DefaultTile.Height);
        }
    }
}
