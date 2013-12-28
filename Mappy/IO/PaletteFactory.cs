namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    using Mappy.Palette;

    public static class PaletteFactory
    {
        private const int TAPaletteColorCount = 256;

        public static CompositePalette FromPal(Stream file)
        {
            PaletteReader r = new PaletteReader(new StreamReader(file));

            if (r.ColorsCount != TAPaletteColorCount)
            {
                throw new ArgumentException("Palette is not " + TAPaletteColorCount + " colors");
            }

            Color[] forward = new Color[r.ColorsCount];
            Dictionary<Color, int> reverse = new Dictionary<Color, int>();

            for (int i = 0; i < r.ColorsCount; i++)
            {
                Color c = r.ReadColor();
                forward[i] = c;
                reverse[c] = i;
            }

            return new CompositePalette(
                new ArrayPalette(forward),
                new DictionaryReversePalette(reverse));
        }
    }
}
