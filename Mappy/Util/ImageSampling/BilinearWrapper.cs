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
            var cellWidth = this.source.Width / (float)this.Width;
            var cellHeight = this.source.Height / (float)this.Height;

            var startX = (int)(x * cellWidth);
            var startY = (int)(y * cellHeight);

            var remX = this.source.Width % this.Width;
            var remY = this.source.Height % this.Height;

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
            var startX = x;
            var startY = y;

            var r = 0;
            var g = 0;
            var b = 0;

            for (var dy = 0; dy < height; dy++)
            {
                for (var dx = 0; dx < width; dx++)
                {
                    var c = this.source.GetPixel(startX + dx, startY + dy);
                    r += c.R;
                    g += c.G;
                    b += c.B;
                }
            }

            var factor = width * height;

            r /= factor;
            g /= factor;
            b /= factor;

            return Color.FromArgb(r, g, b);
        }
    }
}
