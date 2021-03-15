
namespace Mappy.UI.Painters
{
    using System;
    using System.Drawing;
    using Mappy.Collections;
    using Mappy.Util;

    public class HeightGridPainter : IPainter
    {
        private static (float, float, float) HslToRgb(float hue, float saturation, float lightness)
        {
            var c = (1.0f - Math.Abs((2.0f * lightness) - 1.0f)) * saturation;
            var x = c * (1.0f - Math.Abs(((hue * 6.0f) % 2.0f) - 1.0f));
            var m = lightness - (c / 2.0f);

            float rPrime, gPrime, bPrime;

            if (hue < 1.0f / 6.0f)
            {
                (rPrime, gPrime, bPrime) = (c, x, 0.0f);
            }
            else if (hue < 2.0f / 6.0f)
            {
                (rPrime, gPrime, bPrime) = (c, x, 0.0f);
            }
            else if (hue < 3.0f / 6.0f)
            {
                (rPrime, gPrime, bPrime) = (0.0f, c, x);
            }
            else if (hue < 4.0f / 6.0f)
            {
                (rPrime, gPrime, bPrime) = (0.0f, x, c);
            }
            else if (hue < 5.0f / 6.0f)
            {
                (rPrime, gPrime, bPrime) = (x, 0.0f, c);
            }
            else
            {
                (rPrime, gPrime, bPrime) = (c, 0.0f, x);
            }

            return (rPrime + m, gPrime + m, bPrime + m);
        }

        private IGrid<int> heightGrid;
        private int tileSize;

        public HeightGridPainter(IGrid<int> heightGrid, int v)
        {
            this.heightGrid = heightGrid;
            this.tileSize = v;
        }

        public int SeaLevel { get; set; }

        public void Paint(Graphics graphics, Rectangle clipRectangle)
        {
            var startX = Util.Clamp(clipRectangle.Left / this.tileSize, 0, this.heightGrid.Width - 2);
            var startY = Util.Clamp(clipRectangle.Top / this.tileSize, 0, this.heightGrid.Height - 2);

            var endX = Util.Clamp(clipRectangle.Right / this.tileSize, 0, this.heightGrid.Width - 2);
            var endY = Util.Clamp((clipRectangle.Bottom / this.tileSize) + 8, 0, this.heightGrid.Height - 2);

            for (var y = startY; y <= endY; ++y)
            {
                for (var x = startX; x <= endX; ++x)
                {
                    var posX = x * this.tileSize;
                    var posY = (y * this.tileSize) - (this.heightGrid.Get(x, y) / 2);

                    var posX2 = (x + 1) * this.tileSize;
                    var posY2 = (y * this.tileSize) - (this.heightGrid.Get(x + 1, y) / 2);

                    var posX3 = x * this.tileSize;
                    var posY3 = ((y + 1) * this.tileSize) - (this.heightGrid.Get(x, y + 1) / 2);

                    var h = this.heightGrid.Get(x, y);

                    var landHue = 30.0f / 360.0f;
                    var seaHue = 210.0f / 360.0f;

                    var lightness = ((float)h) / 255.0f;

                    var (r, g, b) = HslToRgb(h < this.SeaLevel ? seaHue : landHue, 1.0f, lightness);

                    using (var pen = new Pen(Color.FromArgb(
                        Util.Clamp((int)(r * 255.0f), 0, 255),
                        Util.Clamp((int)(g * 255.0f), 0, 255),
                        Util.Clamp((int)(b * 255.0f), 0, 255))))
                    {
                        graphics.DrawLine(pen, posX, posY, posX2, posY2);
                        graphics.DrawLine(pen, posX, posY, posX3, posY3);
                    }
                }
            }
        }
    }
}
