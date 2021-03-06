namespace Mappy.UI.Drawables
{
    using System.Drawing;

    using Mappy.Data;
    using Mappy.UI.Painters;

    public class DrawableTile : AbstractDrawable
    {
        private readonly IMapTile tile;
        private readonly BitmapGridPainter painter;
        private readonly ContourHeightPainter heightPainter;
        private readonly HeightGridPainter heightGridPainter;

        private bool drawHeightmap;
        private bool drawHeightGrid;

        public DrawableTile(IMapTile tile)
        {
            this.tile = tile;
            this.painter = new BitmapGridPainter(tile.TileGrid, 32);
            this.heightPainter = new ContourHeightPainter(tile.HeightGrid, 16);
            this.heightPainter.ShowSeaLevel = true;

            this.heightGridPainter = new HeightGridPainter(tile.HeightGrid, 16);
        }

        public override Size Size => new Size(this.Width, this.Height);

        public override int Width => this.tile.TileGrid.Width * 32;

        public override int Height => this.tile.TileGrid.Height * 32;

        public Color BackgroundColor
        {
            get => this.painter.BackgroundColor;
            set => this.painter.BackgroundColor = value;
        }

        public bool DrawHeightMap
        {
            get => this.drawHeightmap;

            set
            {
                if (this.drawHeightmap != value)
                {
                    this.drawHeightmap = value;
                    this.OnAreaChanged();
                }
            }
        }

        public bool DrawHeightGrid
        {
            get => this.drawHeightGrid;

            set
            {
                if (this.drawHeightGrid != value)
                {
                    this.drawHeightGrid = value;
                    this.OnAreaChanged();
                }
            }
        }

        public int SeaLevel
        {
            get => this.heightPainter.SeaLevel;

            set
            {
                bool changed = false;
                if (this.heightPainter.SeaLevel != value)
                {
                    this.heightPainter.SeaLevel = value;
                    if (this.drawHeightmap && this.heightPainter.ShowSeaLevel)
                    {
                        changed = true;
                    }
                }

                if (this.heightGridPainter.SeaLevel != value)
                {
                    this.heightGridPainter.SeaLevel = value;
                    if (this.drawHeightGrid)
                    {
                        changed = true;
                    }
                }

                if (changed)
                {
                    this.OnAreaChanged();
                }
            }
        }

        public override void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            this.painter.Paint(graphics, clipRectangle);

            if (this.drawHeightmap)
            {
                this.heightPainter.Paint(graphics, clipRectangle);
            }

            if (this.drawHeightGrid)
            {
                this.heightGridPainter.Paint(graphics, clipRectangle);
            }
        }
    }
}
