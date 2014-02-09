namespace Mappy.Palette
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class ArrayPalette : IPalette
    {
        private readonly Color[] palette;

        public ArrayPalette(Color[] colors)
        {
            this.palette = colors;
        }

        public int Count
        {
            get
            {
                return this.palette.Length;
            }
        }

        Color IPalette.this[int index]
        {
            get
            {
                return this.palette[index];
            }
        }

        public IEnumerator<Color> GetEnumerator()
        {
            foreach (var c in this.palette)
            {
                yield return c;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
