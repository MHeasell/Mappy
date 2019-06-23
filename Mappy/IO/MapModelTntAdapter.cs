namespace Mappy.IO
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Mappy.Collections;
    using Mappy.Models;
    using Mappy.Util;

    using TAUtil.Gdi.Bitmap;
    using TAUtil.Tnt;

    /// <summary>
    /// Adapter for the TNTWriter,
    /// allowing data in a MapModel instance to be written to TNT format.
    /// </summary>
    public class MapModelTntAdapter : ITntSource
    {
        private readonly IReadOnlyMapModel model;

        private readonly Bitmap[] tiles;

        private readonly IDictionary<Bitmap, int> reverseTiles;

        private readonly string[] features;

        private readonly IDictionary<string, int> reverseFeatures;

        public MapModelTntAdapter(IReadOnlyMapModel model)
        {
            this.model = model;
            this.tiles = Util.GetUsedTiles(model.Tile).ToArray();
            this.reverseTiles = Util.ReverseMapping(this.tiles);
            this.features = model.EnumerateFeatureInstances().Select(x => x.FeatureName).Distinct().ToArray();
            this.reverseFeatures = Util.ReverseMapping(this.features);
        }

        public int DataWidth => this.model.Tile.TileGrid.Width;

        public int DataHeight => this.model.Tile.TileGrid.Height;

        public int SeaLevel => this.model.SeaLevel;

        public int TileCount => this.tiles.Length;

        public int AnimCount => this.features.Length;

        public IEnumerable<byte[]> EnumerateTiles()
        {
            return this.tiles.Select(this.ToBytes);
        }

        public IEnumerable<string> EnumerateAnims()
        {
            return this.features;
        }

        public IEnumerable<int> EnumerateData()
        {
            return this.model.Tile.TileGrid.Select(x => this.reverseTiles[x]);
        }

        public IEnumerable<TileAttr> EnumerateAttrs()
        {
            for (var y = 0; y < this.model.Tile.HeightGrid.Height; y++)
            {
                for (var x = 0; x < this.model.Tile.HeightGrid.Width; x++)
                {
                    yield return this.GetAttr(x, y);
                }
            }
        }

        public MinimapInfo GetMinimap()
        {
            return new MinimapInfo(
                this.model.Minimap.Width,
                this.model.Minimap.Height,
                this.ToBytes(this.model.Minimap));
        }

        private byte[] ToBytes(Bitmap tile)
        {
            return BitmapConvert.ToBytes(tile);
        }

        private TileAttr GetAttr(int x, int y)
        {
            var attr = default(TileAttr);

            attr.Height = (byte)this.model.Tile.HeightGrid.Get(x, y);

            if (this.model.Voids.Get(x, y))
            {
                attr.Feature = TileAttr.FeatureVoid;
            }
            else
            {
                var f = this.model.GetFeatureInstanceAt(x, y);
                if (f == null)
                {
                    attr.Feature = TileAttr.FeatureNone;
                }
                else
                {
                    attr.Feature = (ushort)this.reverseFeatures[f.FeatureName];
                }
            }

            return attr;
        }
    }
}
