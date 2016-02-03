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

        public int Width => this.source.Width;

        public int Height => this.source.Height;

        public Color this[int x, int y] => this.palette[this.palette.GetNearest(this.source[x, y])];
    }
}
