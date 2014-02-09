namespace Mappy.Palette
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    public class TransparencyMaskedPalette : IPalette
    {
        private readonly IPalette palette;

        public TransparencyMaskedPalette(IPalette palette, int transparencyIndex = -1)
        {
            this.palette = palette;
            this.TransparencyIndex = transparencyIndex;
        }

        public int TransparencyIndex { get; set; }

        public int Count
        {
            get
            {
                return this.palette.Count;
            }
        }

        public Color this[int index]
        {
            get
            {
                return index == this.TransparencyIndex ? Color.Transparent : this.palette[index];
            }
        }

        public IEnumerator<Color> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
