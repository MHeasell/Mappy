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

        static Globals()
        {
            using (Stream s = new MemoryStream(Mappy.Properties.Resources.TAPalette))
            {
                Color[] arr = Mappy.Util.Palette.LoadArr(s);
                Palette = new ArrayPalette(arr);
                ReversePalette = new DictionaryReversePalette(Mappy.Util.Util.ReverseMapping(arr));
            }

            TileCache = new BitmapCache();
        }
    }
}
