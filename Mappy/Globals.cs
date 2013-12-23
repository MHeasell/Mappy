namespace Mappy
{
    using System.Drawing;
    using System.IO;

    using Mappy.IO;

    public static class Globals
    {
        public static readonly Color[] Palette;

        public static readonly TileManager TileManager;

        static Globals()
        {
            using (Stream s = new MemoryStream(Mappy.Properties.Resources.TAPalette))
            {
                Palette = Mappy.Util.Palette.LoadArr(s);
            }

            TileManager = new TileManager(Palette);
        }
    }
}
