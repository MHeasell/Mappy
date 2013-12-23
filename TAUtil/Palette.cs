namespace TAUtil
{
    using System;
    using System.Drawing;
    using System.IO;

    public static class Palette
    {
        public static Color[] LoadArr(Stream file)
        {
            PaletteReader r = new PaletteReader(new StreamReader(file));

            if (r.ColorsCount != 256)
            {
                throw new ArgumentException("Palette is not 256 colors");
            }

            Color[] pal = new Color[256];

            for (int i = 0; i < 256; i++)
            {
                pal[i] = r.ReadColor();
            }

            return pal;
        }
    }
}
