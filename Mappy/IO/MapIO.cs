namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
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
            var d = new Dictionary<Color, int>();
            for (int i = 0; i < palette.Length; i++)
            {
                d[palette[i]] = i;
            }

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
            WriteMapData(writer, map.Tile.TileGrid, tileMapping);

            h.PtrMapAttr = (uint)s.Position;
            foreach (TileAttr attr in EnumAsTileAttrs(map, featureMapping))
            {
                writer.WriteAttr(attr);
            }

            h.PtrTileGfx = (uint)s.Position;
            WriteTiles(writer, tileMapping, d);

            h.PtrTileAnims = (uint)s.Position;
            foreach (string anim in featureMapping.Select(x => x.Name))
            {
                writer.WriteAnim(anim);
            }

            h.PtrMiniMap = (uint)s.Position;
            writer.WriteMinimap(map.Minimap.Width, map.Minimap.Height, ConvertToBytes(map.Minimap, d));

            h.Unknown1 = 1; // if this is set to 0, the minimap doesn't show

            // skip back and write the header
            s.Seek(0, SeekOrigin.Begin);
            h.Write(s);
        }

        public static void WriteTiles(TntWriter writer, IEnumerable<Bitmap> tiles, IDictionary<Color, int> d)
        {
            foreach (Bitmap tile in tiles)
            {
                writer.WriteTile(ConvertToBytes(tile, d));
            }
        }

        private static byte[] ConvertToBytes(Bitmap tile, IDictionary<Color, int> palette)
        {
            Rectangle r = new Rectangle(0, 0, tile.Width, tile.Height);
            BitmapData data = tile.LockBits(r, ImageLockMode.ReadOnly, tile.PixelFormat);

            int length = tile.Width * tile.Height;

            byte[] output = new byte[length];

            unsafe
            {
                int* pointer = (int*)data.Scan0;
                for (int i = 0; i < length; i++)
                {
                    Color c = Color.FromArgb(pointer[i]);
                    output[i] = (byte)palette[c];
                }
            }

            tile.UnlockBits(data);

            return output;
        }

        private static void WriteMapData(TntWriter writer, IEnumerable<Bitmap> data, Bitmap[] mapping)
        {
            var d = new Dictionary<Bitmap, short>();
            for (short i = 0; i < mapping.Length; i++)
            {
                d[mapping[i]] = i;
            }

            foreach (Bitmap b in data)
            {
                writer.WriteDataCell(d[b]);
            }
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

        private static Bitmap[] ReadTiles(TntReader reader, Color[] palette)
        {
            Bitmap[] bitmaps = new Bitmap[reader.TileCount];

            for (int i = 0; i < reader.TileCount; i++)
            {
                bitmaps[i] = Util.AddTileToDatabase(reader.ReadTile(), palette);
            }

            return bitmaps;
        }

        private static MapModel LoadHelper(TntReader f, Color[] p, IDictionary<string, Feature> featureBank, MapAttributes attrs)
        {
            // create the model
            MapModel model = new MapModel(f.DataWidth, f.DataHeight, attrs);

            // retrieve features
            f.SeekToAnims();
            Feature[] features = ReadFeatures(f, featureBank);

            // retrieve tiles
            f.SeekToTiles();
            Bitmap[] tiles = ReadTiles(f, p);

            // populate model tile data
            f.SeekToData();
            for (int y = 0; y < f.DataHeight; y++)
            {
                for (int x = 0; x < f.DataWidth; x++)
                {
                    model.Tile.TileGrid.Set(x, y, tiles[f.ReadDataCell()]);
                }
            }

            // populate model heights, features, voids
            f.SeekToAttrs();
            for (int y = 0; y < f.Height; y++)
            {
                for (int x = 0; x < f.Width; x++)
                {
                    TileAttr t = f.ReadAttr();

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
                }
            }

            // don't forget the minimap
            f.SeekToMinimap();
            var minimap = f.ReadMinimap();
            model.Minimap = Util.ReadToBitmap(minimap.Data, p, minimap.Width, minimap.Height);

            // and sealevel
            model.SeaLevel = f.SeaLevel;

            return model;
        }

        private static Feature[] ReadFeatures(TntReader reader, IDictionary<string, Feature> bank)
        {
            Feature[] arr = new Feature[reader.AnimCount];

            for (int i = 0; i < reader.AnimCount; i++)
            {
                string name = reader.ReadAnim();
                if (!bank.TryGetValue(name, out arr[i]))
                {
                    arr[i] = new Feature(name, DefaultFrame.Data, DefaultFrame.Offset, new Size(1, 1));
                }
            }

            return arr;
        }
    }
}
