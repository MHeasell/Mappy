namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    using Mappy.Palette;

    /// <summary>
    /// Provides mthods for creating palette instances from files.
    /// </summary>
    public static class PaletteFactory
    {
        private const int TAPaletteColorCount = 256;

        public static IPalette FromPal(Stream file)
        {
            var palette = new ArrayPalette(TAPaletteColorCount);

            PaletteReader r = new PaletteReader(new StreamReader(file));

            if (r.ColorsCount != TAPaletteColorCount)
            {
                throw new ArgumentException("Palette is not " + TAPaletteColorCount + " colors");
            }

            for (int i = 0; i < r.ColorsCount; i++)
            {
                palette[i] = r.ReadColor();
            }

            return palette;
        }
    }
}
