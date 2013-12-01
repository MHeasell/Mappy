namespace Mappy.UI.Drawables
{
    using System.Drawing;
    using Data;
    using Painters;

    public class DrawableTile : IDrawable
    {
        private readonly IMapTile tile;
        private readonly MapPainter painter;
        private readonly ContourHeightPainter heightPainter;

        public DrawableTile(IMapTile tile)
        {
            this.tile = tile;
            this.painter = new MapPainter(tile.TileGrid, 32);
            this.heightPainter = new ContourHeightPainter(tile.HeightGrid, 16);
        }

        public Size Size
        {
            get { return new Size(this.Width, this.Height); }
        }

        public int Width
        {
            get { return this.tile.TileGrid.Width * 32; }
        }

        public int Height
        {
            get { return this.tile.TileGrid.Height * 32; }
        }

        public bool DrawHeightMap { get; set; }

        public void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            this.painter.Paint(graphics, clipRectangle);
            if (this.DrawHeightMap)
            {
                this.heightPainter.Paint(graphics, clipRectangle);
            }
        }
    }
}
