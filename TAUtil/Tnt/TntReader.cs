namespace TAUtil.Tnt
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class TntReader : IDisposable
    {
        public const int TileSize = 32;
        public const byte MinimapVoidByte = 0x64;

        private readonly Stream stream;

        private TntHeader header;

        public TntReader(Stream s)
        {
            TntHeader.Read(s, ref this.header);
            this.stream = s;
        }

        ~TntReader()
        {
            this.Dispose(false);
        }

        public int Width
        {
            get
            {
                return (int)this.header.Width;
            }
        }

        public int Height
        {
            get
            {
                return (int)this.header.Height;
            }
        }

        public int DataWidth
        {
            get
            {
                return this.Width / 2;
            }
        }

        public int DataHeight
        {
            get
            {
                return this.Height / 2;
            }
        }

        public int SeaLevel
        {
            get
            {
                // FIXME: unsafe cast
                return (int)this.header.SeaLevel;
            }
        }

        public IEnumerable<byte[,]> EnumerateTilesBytes()
        {
            this.stream.Seek(this.header.PtrTileGfx, SeekOrigin.Begin);
            for (int i = 0; i < this.header.Tiles; i++)
            {
                yield return Util.LoadMapTile(this.stream);
            }
        }

        public IEnumerable<Bitmap> EnumerateTilesBitmaps(Color[] palette)
        {
            this.stream.Seek(this.header.PtrTileGfx, SeekOrigin.Begin);
            for (int i = 0; i < this.header.Tiles; i++)
            {
                byte[] tile = Util.LoadMapTile1D(this.stream);
                yield return Util.GetBitmap(tile, palette);
            }
        }

        public IEnumerable<int> EnumerateData()
        {
            this.stream.Seek(this.header.PtrMapData, SeekOrigin.Begin);
            BinaryReader b = new BinaryReader(this.stream);

            int elems = this.DataWidth * this.DataHeight;
            for (int i = 0; i < elems; i++)
            {
                yield return b.ReadUInt16();
            }
        }

        public IEnumerable<TileAttr> EnumerateAttrs()
        {
            this.stream.Seek(this.header.PtrMapAttr, SeekOrigin.Begin);
            int elems = this.Width * this.Height;
            for (int i = 0; i < elems; i++)
            {
                yield return TileAttr.Read(this.stream);
            }
        }

        public byte[][,] GetTilesBytes()
        {
            byte[][,] tiles = new byte[this.header.Tiles][,];
            int i = 0;
            foreach (byte[,] t in this.EnumerateTilesBytes())
            {
                tiles[i] = t;
                i++;
            }

            return tiles;
        }

        public Bitmap[] GetTileBitmaps(Color[] palette)
        {
            Bitmap[] tiles = new Bitmap[this.header.Tiles];
            int i = 0;
            foreach (Bitmap b in this.EnumerateTilesBitmaps(palette))
            {
                tiles[i] = b;
                i++;
            }

            return tiles;
        }

        public TileAttr[,] GetAttrs()
        {
            TileAttr[,] a = new TileAttr[this.Height, this.Width];
            IEnumerator<TileAttr> attrs = this.EnumerateAttrs().GetEnumerator();
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    attrs.MoveNext();
                    a[y, x] = attrs.Current;
                }
            }

            return a;
        }

        public byte[,][,] GetTileDataBytes()
        {
            byte[][,] tiles = this.GetTilesBytes();

            IEnumerator<int> data = this.EnumerateData().GetEnumerator();

            byte[,][,] map = new byte[this.DataHeight, this.DataWidth][,];

            for (int y = 0; y < this.DataHeight; y++)
            {
                for (int x = 0; x < this.DataWidth; x++)
                {
                    data.MoveNext();
                    map[y, x] = tiles[data.Current];
                }
            }

            return map;
        }

        public Bitmap[,] GetTileDataBitmaps(Color[] palette)
        {
            Bitmap[] tiles = this.GetTileBitmaps(palette);
            IEnumerator<int> data = this.EnumerateData().GetEnumerator();

            Bitmap[,] map = new Bitmap[this.DataHeight, this.DataWidth];
            for (int y = 0; y < this.DataHeight; y++)
            {
                for (int x = 0; x < this.DataWidth; x++)
                {
                    data.MoveNext();
                    map[y, x] = tiles[data.Current];
                }
            }

            return map;
        }

        public Bitmap GetMinimap(Color[] p)
        {
            this.stream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
            BinaryReader rb = new BinaryReader(this.stream);

            int readWidth = rb.ReadInt32();
            int readHeight = rb.ReadInt32();

            byte[,] map = new byte[readHeight, readWidth];

            for (int y = 0; y < readHeight; y++)
            {
                for (int x = 0; x < readWidth; x++)
                {
                    map[y, x] = rb.ReadByte();
                }
            }

            return TntReader.TrimMinimap(map, p);
        }

        public IEnumerable<string> EnumerateFeatureNames()
        {
            this.stream.Seek(this.header.PtrTileAnims, SeekOrigin.Begin);
            BinaryReader b = new BinaryReader(this.stream);
            for (int i = 0; i < this.header.TileAnims; i++)
            {
                b.ReadUInt32();
                byte[] chars = b.ReadBytes(128);
                string s = Util.ConvertChars(chars);
                yield return s;
            }
        }

        public string[] GetFeatureNames()
        {
            string[] arr = new string[this.header.TileAnims];
            int i = 0;
            foreach (string name in this.EnumerateFeatureNames())
            {
                arr[i] = name;
                i++;
            }

            return arr;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.stream.Close();
            }
        }

        private static Bitmap TrimMinimap(byte[,] minimap, Color[] palette)
        {
            Size realSize = TntReader.GetMinimapActualSize(minimap);

            Bitmap newMinimap = new Bitmap(realSize.Width, realSize.Height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, realSize.Width, realSize.Height);
            BitmapData data = newMinimap.LockBits(rect, ImageLockMode.WriteOnly, newMinimap.PixelFormat);

            unsafe
            {
                byte* pointer = (byte*)data.Scan0;
                for (int y = 0; y < realSize.Height; y++)
                {
                    for (int x = 0; x < realSize.Width; x++)
                    {
                        Color c = palette[minimap[y, x]];
                        int pos = (y * realSize.Width * 4) + (x * 4);
                        pointer[pos] = c.B;
                        pointer[pos + 1] = c.G;
                        pointer[pos + 2] = c.R;
                        pointer[pos + 3] = c.A;
                    }
                }
            }
            
            newMinimap.UnlockBits(data);

            return newMinimap;
        }

        private static Size GetMinimapActualSize(byte[,] minimap)
        {
            int rows = 0;
            int cols = 0;

            for (int i = 0; i < minimap.GetLength(0); i++)
            {
                if (minimap[i, 0] == TntReader.MinimapVoidByte)
                {
                    break;
                }

                rows++;
            }

            for (int i = 0; i < minimap.GetLength(1); i++)
            {
                if (minimap[0, i] == TntReader.MinimapVoidByte)
                {
                    break;
                }

                cols++;
            }

            return new Size(cols, rows);
        }
    }
}
