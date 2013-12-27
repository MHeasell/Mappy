namespace Mappy
{
    using System.Drawing;
    using System.IO;

    using Mappy.IO;

    public static class Globals
    {
        public static readonly Color[] Palette;

        public static readonly BitmapCache TileCache;

        static Globals()
        {
            using (Stream s = new MemoryStream(Mappy.Properties.Resources.TAPalette))
            {
                Palette = Mappy.Util.Palette.LoadArr(s);
            }

            TileCache = new BitmapCache();
        }
    }
}
