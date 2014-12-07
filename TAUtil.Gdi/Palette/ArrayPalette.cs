namespace TAUtil.Gdi.Palette
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class ArrayPalette : IPalette
    {
        private readonly Color[] palette;

        private readonly Dictionary<Color, int> reversePalette;

        public ArrayPalette(int size)
        {
            this.palette = new Color[size];
            this.reversePalette = new Dictionary<Color, int>();
        }

        public int Count
        {
            get
            {
                return this.palette.Length;
            }
        }

        public Color this[int index]
        {
            get
            {
                return this.palette[index];
            }

            set
            {
                this.palette[index] = value;
                this.reversePalette[value] = index;
            }
        }

        public int LookUp(Color color)
        {
            return this.reversePalette[color];
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
