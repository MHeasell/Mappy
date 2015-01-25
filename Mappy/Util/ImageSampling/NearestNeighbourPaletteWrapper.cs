namespace Mappy.Util.ImageSampling
{
    using System.Drawing;

    using TAUtil.Gdi.Palette;

    public class NearestNeighbourPaletteWrapper : IPixelImage
    {
        private readonly IPixelImage source;

        private readonly IPalette palette;

        public NearestNeighbourPaletteWrapper(IPixelImage source, IPalette palette)
        {
            this.source = source;
            this.palette = palette;
        }

        public int Width
        {
            get
            {
                return this.source.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.source.Height;
            }
        }

        public Color this[int x, int y]
        {
            get
            {
                return this.palette[this.palette.GetNearest(this.source[x, y])];
            }
        }
    }
}
