namespace Mappy
{
    using System.Drawing;
    using System.IO;

    using Mappy.IO;
    using Mappy.Palette;

    public static class Globals
    {
        public static readonly IPalette Palette;

        public static readonly IReversePalette ReversePalette;

        public static readonly BitmapCache TileCache;

        public static readonly Bitmap DefaultTile;

        static Globals()
        {
            using (Stream s = new MemoryStream(Mappy.Properties.Resources.TAPalette))
            {
                var palette = PaletteFactory.FromPal(s);
                Palette = palette.ForwardPalette;
                ReversePalette = palette.ReversePalette;
            }

            TileCache = new BitmapCache();

            DefaultTile = new Bitmap(32, 32);
            var g = Graphics.FromImage(DefaultTile);
            g.FillRectangle(Brushes.White, 0, 0, DefaultTile.Width, DefaultTile.Height);
        }
    }
}
