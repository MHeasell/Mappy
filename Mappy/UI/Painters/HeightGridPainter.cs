
namespace Mappy.UI.Painters
{
    using System.Drawing;
    using Mappy.Collections;
    using Mappy.Util;

    public class HeightGridPainter : IPainter
    {
        private IGrid<int> heightGrid;
        private int tileSize;

        public HeightGridPainter(IGrid<int> heightGrid, int v)
        {
            this.heightGrid = heightGrid;
            this.tileSize = v;
        }

        public void Paint(Graphics g, Rectangle clipRectangle)
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

                    using (var pen = new Pen(Color.FromArgb(h, h, h)))
                    {
                        g.DrawLine(pen, posX, posY, posX2, posY2);
                        g.DrawLine(pen, posX, posY, posX3, posY3);
                    }
                }
            }
        }
    }
}
