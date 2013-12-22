namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Models;
    using Mappy.Util;

    using TAUtil.Gaf;
    using TAUtil.Tdf;
    using TAUtil.Tnt;

    public static class MapIO
    {
        private static readonly GafFrame DefaultFrame;

        static MapIO()
        {
            MapIO.DefaultFrame.Offset = new Point(0, 0);
            MapIO.DefaultFrame.Data = Mappy.Properties.Resources.nofeature;
        }

        public static MapModel Load(TntReader f, Color[] p, IDictionary<string, Feature> featureBank)
        {
            return LoadHelper(f, p, featureBank, new MapAttributes());
        }

        public static MapModel Load(TntReader f, TdfNode n, Color[] p, IDictionary<string, Feature> featureBank)
        {
            // load attrs
            MapAttributes attrs = MapAttributes.Load(n);

            return LoadHelper(f, p, featureBank, attrs);
        }

        public static void WriteTnt(MapModel map, TntWriter writer, Color[] palette)
        {
            TntHeader h = new TntHeader();
            h.IdVersion = TntHeader.TntMagicNumber;
            
            // FIXME: potentially unsafe cast?
            h.Width = (uint)map.Tile.HeightGrid.Width;
            h.Height = (uint)map.Tile.HeightGrid.Height;
            h.SeaLevel = (uint)map.SeaLevel;

            Bitmap[] tileMapping = Util.GetUsedTiles(map.Tile).ToArray();
            h.Tiles = (uint)tileMapping.Length; // FIXME: conversion

            Feature[] featureMapping = map.Features.Values.Distinct().ToArray();
            h.TileAnims = (uint)featureMapping.Length; // FIXME: conversion

            Stream s = writer.BaseStream;

            // skip past the header to start writing
            s.Seek(TntHeader.HeaderLength, SeekOrigin.Begin);

            // write all the data
            h.PtrMapData = (uint)s.Position;
            writer.WriteMapData(map.Tile.TileGrid, tileMapping);

            h.PtrMapAttr = (uint)s.Position;
            writer.WriteAttrs(EnumAsTileAttrs(map, featureMapping));

            h.PtrTileGfx = (uint)s.Position;
            writer.WriteTiles(tileMapping, palette);

            h.PtrTileAnims = (uint)s.Position;
            writer.WriteAnimNames(featureMapping.Select(x => x.Name));

            h.PtrMiniMap = (uint)s.Position;
            writer.WriteMinimap(map.Minimap, palette);

            h.Unknown1 = 1; // if this is set to 0, the minimap doesn't show

            // skip back and write the header
            s.Seek(0, SeekOrigin.Begin);
            h.Write(s);
        }

        private static IEnumerable<TileAttr> EnumAsTileAttrs(MapModel map, Feature[] featureMapping)
        {
            for (int y = 0; y < map.Tile.HeightGrid.Height; y++)
            {
                for (int x = 0; x < map.Tile.HeightGrid.Width; x++)
                {
                    TileAttr attr = new TileAttr();
                    attr.Height = (byte)map.Tile.HeightGrid.Get(x, y);
                    Feature f;
                    if (map.Features.TryGetValue(x, y, out f))
                    {
                        int i = Array.IndexOf(featureMapping, f);
                        Debug.Assert(i != -1, "feature not found in mapping");
                        attr.Feature = (ushort)i;
                    }
                    else if (map.Voids.HasValue(x, y))
                    {
                        attr.Feature = TileAttr.FeatureVoid;
                    }
                    else
                    {
                        attr.Feature = TileAttr.FeatureNone;
                    }

                    yield return attr;
                }
            }
        }

        private static MapModel LoadHelper(TntReader f, Color[] p, IDictionary<string, Feature> featureBank, MapAttributes attrs)
        {
            // create the model
            MapModel model = new MapModel(f.DataWidth, f.DataHeight, attrs);

            // retrieve reference data
            Feature[] features = RetrieveFeatures(f.GetFeatureNames(), featureBank);
            Bitmap[] tiles = f.GetTileBitmaps(p);

            // populate model tile data
            int count = 0;
            foreach (int i in f.EnumerateData())
            {
                model.Tile.TileGrid.Set(count % f.DataWidth, count / f.DataWidth, tiles[i]);
                count++;
            }

            // populate model heights, features, voids
            count = 0;
            foreach (TileAttr t in f.EnumerateAttrs())
            {
                int x = count % f.Width;
                int y = count / f.Width;

                model.Tile.HeightGrid.Set(x, y, t.Height);

                switch (t.Feature)
                {
                    case TileAttr.FeatureNone:
                    case TileAttr.FeatureUnknown:
                        break;
                    case TileAttr.FeatureVoid:
                        model.Voids.Set(x, y, true);
                        break;
                    default:
                        model.Features.Set(x, y, features[t.Feature]);
                        break;
                }

                count++;
            }

            // don't forget the minimap
            model.Minimap = f.GetMinimap(p);

            // and sealevel
            model.SeaLevel = f.SeaLevel;

            return model;
        }

        private static Feature[] RetrieveFeatures(string[] names, IDictionary<string, Feature> bank)
        {
            Feature[] arr = new Feature[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                if (!bank.TryGetValue(names[i], out arr[i]))
                {
                    arr[i] = new Feature(names[i], DefaultFrame.Data, DefaultFrame.Offset, new Size(1, 1));
                }
            }

            return arr;
        }
    }
}
