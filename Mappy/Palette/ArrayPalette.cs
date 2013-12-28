namespace Mappy.Palette
{
    using System.Drawing;

    public class ArrayPalette : IPalette
    {
        private readonly Color[] palette;

        public ArrayPalette(Color[] colors)
        {
            this.palette = colors;
        }

        Color IPalette.this[int index]
        {
            get
            {
                return this.palette[index];
            }
        }
    }
}
