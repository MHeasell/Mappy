namespace Mappy.UI.Drawables
{
    using System.Drawing;

    using Mappy.Data;
    using Mappy.UI.Painters;

    public class DrawableTile : IDrawable
    {
        private readonly IMapTile tile;
        private readonly BitmapGridPainter painter;
        private readonly ContourHeightPainter heightPainter;

        public DrawableTile(IMapTile tile)
        {
            this.tile = tile;
            this.painter = new BitmapGridPainter(tile.TileGrid, 32);
            this.heightPainter = new ContourHeightPainter(tile.HeightGrid, 16);
            this.heightPainter.ShowSeaLevel = true;
        }

        public Size Size => new Size(this.Width, this.Height);

        public int Width => this.tile.TileGrid.Width * 32;

        public int Height => this.tile.TileGrid.Height * 32;

        public Color BackgroundColor
        {
            get
            {
                return this.painter.BackgroundColor;
            }

            set
            {
                this.painter.BackgroundColor = value;
            }
        }

        public bool DrawHeightMap { get; set; }

        public int SeaLevel
        {
            get
            {
                return this.heightPainter.SeaLevel;
            }

            set
            {
                this.heightPainter.SeaLevel = value;
            }
        }

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
