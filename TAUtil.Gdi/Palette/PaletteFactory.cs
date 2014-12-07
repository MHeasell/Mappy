namespace TAUtil.Gdi.Palette
{
    using System.IO;

    /// <summary>
    /// Provides mthods for creating palette instances from files.
    /// </summary>
    public static class PaletteFactory
    {
        private const int TAPaletteColorCount = 256;

        public static IPalette FromBinaryPal(Stream file)
        {
            var p = new ArrayPalette(TAPaletteColorCount);
            var r = new BinaryPaletteReader(new BinaryReader(file));

            for (int i = 0; i < TAPaletteColorCount; i++)
            {
                p[i] = r.ReadColor();
            }

            return p;
        }
    }
}
