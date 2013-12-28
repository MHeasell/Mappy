namespace Mappy
{
    using System.IO;

    using Mappy.IO;
    using Mappy.Palette;

    public static class Globals
    {
        public static readonly IPalette Palette;

        public static readonly IReversePalette ReversePalette;

        public static readonly BitmapCache TileCache;

        static Globals()
        {
            using (Stream s = new MemoryStream(Mappy.Properties.Resources.TAPalette))
            {
                var palette = PaletteFactory.FromPal(s);
                Palette = palette.ForwardPalette;
                ReversePalette = palette.ReversePalette;
            }

            TileCache = new BitmapCache();
        }
    }
}
