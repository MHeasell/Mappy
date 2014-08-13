namespace Mappy.Util.ImageSampling
{
    using System.Collections.Generic;
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

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Color this[int x, int y]
        {
            get
            {
                var cellWidth = this.source.Width / this.Width;
                var cellHeight = this.source.Height / this.Height;

                var startX = x * cellWidth;
                var startY = y * cellHeight;

                var sampledColor = this.SampleArea(startX, startY, cellWidth, cellHeight);
                var nearestNeighbour = NearestNeighbour(sampledColor, Globals.Palette);

                return nearestNeighbour;
            }
        }

        private static Color NearestNeighbour(Color color, IEnumerable<Color> choices)
        {
            Color winner = new Color();
            double winningValue = double.PositiveInfinity;

            foreach (var candidate in choices)
            {
                double dist = DistanceSquared(color, candidate);
                if (dist < winningValue)
                {
                    winner = candidate;
                    winningValue = dist;
                }
            }

            return winner;
        }

        private static double DistanceSquared(Color a, Color b)
        {
            int dR = b.R - a.R;
            int dG = b.G - a.G;
            int dB = b.B - a.B;

            return (dR * dR) + (dG * dG) + (dB * dB);
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
                    Color c = this.source[startX + dx, startY + dy];
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
