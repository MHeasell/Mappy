namespace Mappy.Util.ImageSampling
{
    using System.Drawing;

    public class NearestNeighbourWrapper : IPixelImage
    {
        private readonly IPixelImage source;

        public NearestNeighbourWrapper(IPixelImage source, int width, int height)
        {
            this.source = source;
            this.Width = width;
            this.Height = height;
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Color this[int x, int y]
        {
            get
            {
                int imageX = (int)((x / (float)this.Width) * this.source.Width);
                int imageY = (int)((y / (float)this.Height) * this.source.Height);
                return this.source[imageX, imageY];
            }
        }
    }
}
