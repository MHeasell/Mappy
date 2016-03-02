namespace Mappy.Util.ImageSampling
{
    using System.Drawing;

    public class BilinearWrapper : IPixelImage
    {
        private readonly IPixelImage source;

        public BilinearWrapper(IPixelImage source, int width, int height)
        {
            this.source = source;
            this.Width = width;
            this.Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public Color GetPixel(int x, int y)
        {
            var rect = this.GetRect(x, y);
            var sampledColor = this.SampleArea(rect.X, rect.Y, rect.Width, rect.Height);

            return sampledColor;
        }

        private Rectangle GetRect(int x, int y)
        {
            float cellWidth = this.source.Width / (float)this.Width;
            float cellHeight = this.source.Height / (float)this.Height;

            int startX = (int)(x * cellWidth);
            int startY = (int)(y * cellHeight);

            int remX = this.source.Width % this.Width;
            int remY = this.source.Height % this.Height;

            if (remX > x)
            {
                cellWidth++;
            }

            if (remY > y)
            {
                cellHeight++;
            }

            return new Rectangle(
                startX,
                startY,
                (int)cellWidth + (remX > x ? 1 : 0),
                (int)cellHeight + (remX > y ? 1 : 0));
        }

        private Color SampleArea(int x, int y, int width, int height)
        {
            int startX = x;
            int startY = y;

            int r = 0;
            int g = 0;
            int b = 0;

            for (int dy = 0; dy < height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    Color c = this.source.GetPixel(startX + dx, startY + dy);
                    r += c.R;
                    g += c.G;
                    b += c.B;
                }
            }

            int factor = width * height;

            r /= factor;
            g /= factor;
            b /= factor;

            return Color.FromArgb(r, g, b);
        }
    }
}
