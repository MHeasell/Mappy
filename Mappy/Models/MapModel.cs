namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using Data;
    using Grids;

    using TAUtil.Gaf;
    using TAUtil.Tdf;
    using TAUtil.Tnt;

    using Util;

    public class MapModel : IMapModel
    {
        private static readonly GafFrame DefaultFrame;

        static MapModel()
        {
            MapModel.DefaultFrame.Offset = new Point(0, 0);
            MapModel.DefaultFrame.Data = Mappy.Properties.Resources.nofeature;
        }

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

        public static MapModel Load(TntFile f, Color[] p, IDictionary<string, Feature> featureBank)
        {
            return LoadHelper(f, p, featureBank, new MapAttributes());
        }

        public static MapModel Load(TntFile f, TdfNode n, Color[] p, IDictionary<string, Feature> featureBank)
        {
            // load attrs
            MapAttributes attrs = MapAttributes.Load(n);

            return LoadHelper(f, p, featureBank, attrs);
        }

        public void WriteTnt(Stream s, Color[] palette)
        {
            TntHeader h = new TntHeader();
            h.IdVersion = TntHeader.TntMagicNumber;
            
            // FIXME: potentially unsafe cast?
            h.Width = (uint)this.Tile.HeightGrid.Width;
            h.Height = (uint)this.Tile.HeightGrid.Height;
            h.SeaLevel = (uint)this.SeaLevel;

            Bitmap[] tileMapping = Util.GetUsedTiles(this.Tile).ToArray();
            h.Tiles = (uint)tileMapping.Length; // FIXME: conversion

            Feature[] featureMapping = this.GetUsedFeatures().ToArray();
            h.TileAnims = (uint)featureMapping.Length; // FIXME: conversion

            // skip past the header to start writing
            s.Seek(TntHeader.HeaderLength, SeekOrigin.Begin);

            TntWriter writer = new TntWriter(s);

            // write all the data
            h.PtrMapData = (uint)s.Position;
            writer.WriteMapData(this.Tile.TileGrid, tileMapping);

            h.PtrMapAttr = (uint)s.Position;
            writer.WriteAttrs(this.EnumAsTileAttrs(featureMapping));

            h.PtrTileGfx = (uint)s.Position;
            writer.WriteTiles(tileMapping, palette);

            h.PtrTileAnims = (uint)s.Position;
            writer.WriteAnimNames(MapModel.EnumerateNames(featureMapping));

            h.PtrMiniMap = (uint)s.Position;
            writer.WriteMinimap(this.Minimap, palette);

            h.Unknown1 = 1; // if this is set to 0, the minimap doesn't show

            // skip back and write the header
            s.Seek(0, SeekOrigin.Begin);
            h.Write(s);
        }

        public Bitmap GenerateMinimap()
        {
            int mapWidth = this.Tile.TileGrid.Width * 32;
            int mapHeight = this.Tile.TileGrid.Height * 32;

            int width, height;

            if (this.Tile.TileGrid.Width > this.Tile.TileGrid.Height)
            {
                width = 252;
                height = (int)(252 * (mapHeight / (float)mapWidth));
            }
            else
            {
                height = 252;
                width = (int)(252 * (mapWidth / (float)mapHeight));
            }

            Bitmap b = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int imageX = (int)((x / (float)width) * mapWidth);
                    int imageY = (int)((y / (float)height) * mapHeight);
                    b.SetPixel(x, y, this.GetPixel(imageX, imageY));
                }
            }

            return b;
        }

        private static MapModel LoadHelper(TntFile f, Color[] p, IDictionary<string, Feature> featureBank, MapAttributes attrs)
        {
            // create the model
            MapModel model = new MapModel(f.DataWidth, f.DataHeight, attrs);

            // retrieve reference data
            Feature[] features = MapModel.RetrieveFeatures(f.GetFeatureNames(), featureBank);
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

        private static IEnumerable<string> EnumerateNames(IEnumerable<Feature> features)
        {
            foreach (Feature f in features)
            {
                yield return f.Name;
            }
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

        private IEnumerable<TileAttr> EnumAsTileAttrs(Feature[] featureMapping)
        {
            for (int y = 0; y < this.Tile.HeightGrid.Height; y++)
            {
                for (int x = 0; x < this.Tile.HeightGrid.Width; x++)
                {
                    TileAttr attr = new TileAttr();
                    attr.Height = (byte)this.Tile.HeightGrid.Get(x, y);
                    Feature f;
                    if (this.Features.TryGetValue(x, y, out f))
                    {
                        int i = Array.IndexOf(featureMapping, f);
                        Debug.Assert(i != -1, "feature not found in mapping");
                        attr.Feature = (ushort)i;
                    }
                    else if (this.Voids.HasValue(x, y))
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

        private HashSet<Feature> GetUsedFeatures()
        {
            HashSet<Feature> set = new HashSet<Feature>();
            foreach (Feature f in this.Features.Values)
            {
                set.Add(f);
            }

            return set;
        }

        private Color GetPixel(int x, int y)
        {
            int tileX = x / 32;
            int tileY = y / 32;

            int tilePixelX = x % 32;
            int tilePixelY = y % 32;

            foreach (Positioned<IMapTile> t in this.FloatingTiles.Reverse())
            {
                Rectangle r = new Rectangle(t.Location, new Size(t.Item.TileGrid.Width, t.Item.TileGrid.Height));
                if (r.Contains(tileX, tileY))
                {
                    return t.Item.TileGrid.Get(tileX - t.Location.X, tileY - t.Location.Y).GetPixel(tilePixelX, tilePixelY);
                }
            }

            return this.Tile.TileGrid.Get(tileX, tileY).GetPixel(tilePixelX, tilePixelY);
        }
    }
}
