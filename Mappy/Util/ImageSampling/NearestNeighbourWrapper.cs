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
                // sample at the centre of each pixel
                float ax = x + 0.5f;
                float ay = y + 0.5f;

                int imageX = (int)((ax / this.Width) * this.source.Width);
                int imageY = (int)((ay / this.Height) * this.source.Height);
                return this.source[imageX, imageY];
            }
        }
    }
}
