namespace Mappy.Palette
{
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

        public Color this[int index]
        {
            get
            {
                return index == this.TransparencyIndex ? Color.Transparent : this.palette[index];
            }
        }
    }
}
