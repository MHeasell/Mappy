namespace Mappy.Models
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    using Data;

    using Mappy.Collections;

    public class MapModel : IMapModel
    {
        public MapModel(int width, int height)
            : this(width, height, new MapAttributes())
        {
        }

        public MapModel(int width, int height, MapAttributes attrs)
            : this(new MapTile(width, height), attrs)
        {
        }

        public MapModel(MapTile tile)
            : this(tile, new MapAttributes())
        {
        }

        public MapModel(MapTile tile, MapAttributes attrs)
        {
            this.Tile = tile;
            this.Attributes = attrs;
            this.FloatingTiles = new List<Positioned<IMapTile>>();
            this.Features = new SparseGrid<Feature>(this.Tile.HeightGrid.Width, this.Tile.HeightGrid.Height);
            this.Voids = new SparseGrid<bool>(this.Tile.HeightGrid.Width, this.Tile.HeightGrid.Height);
            this.Minimap = new Bitmap(252, 252);
            var g = Graphics.FromImage(this.Minimap);
            g.FillRectangle(Brushes.White, 0, 0, this.Minimap.Width, this.Minimap.Height);
        }

        public MapAttributes Attributes { get; private set; }

        public IMapTile Tile { get; private set; }

        public IList<Positioned<IMapTile>> FloatingTiles { get; private set; }

        public ISparseGrid<Feature> Features
        {
            get;
            private set;
        }

        public ISparseGrid<bool> Voids
        {
            get;
            private set;
        }

        public int SeaLevel { get; set; }

        public Bitmap Minimap { get; set; }
    }
}
