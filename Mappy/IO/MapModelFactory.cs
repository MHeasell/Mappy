namespace Mappy.IO
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Models;
    using Mappy.Palette;

    using TAUtil;
    using TAUtil.Tnt;

    public class MapModelFactory
    {
        private readonly BitmapDeserializer bitmapDeserializer;

        private readonly IDictionary<string, Feature> featureBank;

        private readonly Bitmap defaultFeatureImage;

        public MapModelFactory(
            IPalette palette,
            IDictionary<string, Feature> bank,
            Bitmap defaultFeatureImage)
        {
            this.bitmapDeserializer = new BitmapDeserializer(palette);
            this.featureBank = bank;
            this.defaultFeatureImage = defaultFeatureImage;
        }

        public MapModel FromTntAndOta(ITntSource tnt, MapAttributes ota)
        {
            MapModel m = new MapModel(tnt.DataWidth, tnt.DataHeight, ota);

            return this.ReadTnt(tnt, m);
        }

        public MapModel FromTnt(ITntSource tnt)
        {
            MapModel m = new MapModel(tnt.DataWidth, tnt.DataHeight);

            return this.ReadTnt(tnt, m);
        }

        private static void ReadFeatures(ITntSource tnt, MapModel model, List<Feature> features)
        {
            var enumer = tnt.EnumerateAttrs().GetEnumerator();
            for (int y = 0; y < tnt.DataHeight * 2; y++)
            {
                for (int x = 0; x < tnt.DataWidth * 2; x++)
                {
                    enumer.MoveNext();
                    model.Tile.HeightGrid.Set(x, y, enumer.Current.Height);

                    switch (enumer.Current.Feature)
                    {
                        case TileAttr.FeatureNone:
                        case TileAttr.FeatureUnknown:
                            break;
                        case TileAttr.FeatureVoid:
                            model.Voids.Set(x, y, true);
                            break;
                        default:
                            model.Features.Set(x, y, features[enumer.Current.Feature]);
                            break;
                    }
                }
            }
        }

        private static void ReadData(ITntSource tnt, MapModel model, List<Bitmap> tiles)
        {
            var enumer = tnt.EnumerateData().GetEnumerator();
            for (int y = 0; y < tnt.DataHeight; y++)
            {
                for (int x = 0; x < tnt.DataWidth; x++)
                {
                    enumer.MoveNext();
                    model.Tile.TileGrid.Set(x, y, tiles[enumer.Current]);
                }
            }
        }

        private MapModel ReadTnt(ITntSource tnt, MapModel model)
        {
            List<Bitmap> tiles = new List<Bitmap>(tnt.TileCount);
            tiles.AddRange(tnt.EnumerateTiles().Select(this.ToBitmap));

            ReadData(tnt, model, tiles);

            List<Feature> features = new List<Feature>(tnt.AnimCount);
            features.AddRange(tnt.EnumerateAnims().Select(this.ToFeature));

            ReadFeatures(tnt, model, features);

            model.SeaLevel = tnt.SeaLevel;

            model.Minimap = this.ToBitmap(tnt.GetMinimap());

            return model;
        }

        private Feature ToFeature(string name)
        {
            Feature f;
            if (!this.featureBank.TryGetValue(name, out f))
            {
                f = new Feature(
                    name,
                    this.defaultFeatureImage,
                    new Point(0, 0),
                    new Size(1, 1));
            }

            return f;
        }

        private Bitmap ToBitmap(byte[] tile)
        {
            Bitmap bmp = this.bitmapDeserializer.Deserialize(tile, MapConstants.TileWidth, MapConstants.TileHeight);
            return Globals.TileCache.GetOrAddBitmap(bmp);
        }

        private Bitmap ToBitmap(MinimapInfo minimap)
        {
            return this.bitmapDeserializer.Deserialize(minimap.Data, minimap.Width, minimap.Height);
        }
    }
}
