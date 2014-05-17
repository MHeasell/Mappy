namespace Mappy.IO
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Models;
    using Mappy.Palette;
    using Mappy.Util;

    using TAUtil.Tnt;

    /// <summary>
    /// Adapter for the TNTWriter,
    /// allowing data in a MapModel instance to be written to TNT format.
    /// </summary>
    public class MapModelTntAdapter : ITntSource
    {
        private readonly IMapModel model;

        private readonly Bitmap[] tiles;

        private readonly IDictionary<Bitmap, int> reverseTiles;

        private readonly Feature[] features;

        private readonly IDictionary<Feature, int> reverseFeatures;

        private readonly BitmapSerializer bitmapSerializer;

        public MapModelTntAdapter(IMapModel model, IReversePalette reversePalette)
        {
            this.model = model;
            this.tiles = Util.GetUsedTiles(model.Tile).ToArray();
            this.reverseTiles = Util.ReverseMapping(this.tiles);
            this.features = model.Features.Values.Distinct().ToArray();
            this.reverseFeatures = Util.ReverseMapping(this.features);
            this.bitmapSerializer = new BitmapSerializer(reversePalette);
        }

        public int DataWidth
        {
            get
            {
                return this.model.Tile.TileGrid.Width;
            }
        }

        public int DataHeight
        {
            get
            {
                return this.model.Tile.TileGrid.Height;
            }
        }

        public int SeaLevel
        {
            get
            {
                return this.model.SeaLevel;
            }
        }

        public int TileCount
        {
            get
            {
                return this.tiles.Length;
            }
        }

        public int AnimCount
        {
            get
            {
                return this.features.Length;
            }
        }

        public IEnumerable<byte[]> EnumerateTiles()
        {
            return this.tiles.Select(this.ToBytes);
        }

        public IEnumerable<string> EnumerateAnims()
        {
            return this.features.Select(x => x.Name);
        }

        public IEnumerable<int> EnumerateData()
        {
            return this.model.Tile.TileGrid.Select(x => this.reverseTiles[x]);
        }

        public IEnumerable<TileAttr> EnumerateAttrs()
        {
            for (int y = 0; y < this.model.Tile.HeightGrid.Height; y++)
            {
                for (int x = 0; x < this.model.Tile.HeightGrid.Width; x++)
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
            byte[] bytes = new byte[tile.Width * tile.Height];
            MemoryStream s = new MemoryStream(bytes, true);
            this.bitmapSerializer.Serialize(s, tile);
            return bytes;
        }

        private TileAttr GetAttr(int x, int y)
        {
            TileAttr attr = new TileAttr();

            attr.Height = (byte)this.model.Tile.HeightGrid[x, y];

            if (this.model.Voids[x, y])
            {
                attr.Feature = TileAttr.FeatureVoid;
            }
            else
            {
                Feature f;
                if (this.model.Features.TryGetValue(x, y, out f))
                {
                    attr.Feature = (ushort)this.reverseFeatures[f];
                }
                else
                {
                    attr.Feature = TileAttr.FeatureNone;
                }
            }

            return attr;
        }
    }
}
