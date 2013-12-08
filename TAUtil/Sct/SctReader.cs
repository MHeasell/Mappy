namespace TAUtil.Sct
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    using TAUtil.Tnt;

    public class SctReader : IDisposable
    {
        private readonly BinaryReader reader;

        private SctHeader header;

        public SctReader(Stream s)
        {
            this.reader = new BinaryReader(s);
            SctHeader.Read(this.reader, ref this.header);
        }

        public int Width
        {
            get { return (int)this.header.Width; }
        }

        public int Height
        {
            get { return (int)this.header.Height; }
        }

        public IEnumerable<byte[]> Tiles
        {
            get
            {
                this.reader.BaseStream.Seek(this.header.PtrTiles, SeekOrigin.Begin);
                for (int i = 0; i < this.header.Tiles; i++)
                {
                    byte[] tile = Util.LoadMapTile1D(this.reader);
                    yield return tile;
                }
            }
        }

        public IEnumerable<int> Data
        {
            get
            {
                this.reader.BaseStream.Seek(this.header.PtrData, SeekOrigin.Begin);
                int elems = this.Width * this.Height;
                for (int i = 0; i < elems; i++)
                {
                    yield return this.reader.ReadUInt16();
                }
            }
        }

        public IEnumerable<TileAttr> Attrs
        {
            get
            {
                this.reader.BaseStream.Seek(this.header.PtrHeightData, SeekOrigin.Begin);
                int elems = (this.Width * 2) * (this.Height * 2);
                for (int i = 0; i < elems; i++)
                {
                    yield return TileAttr.ReadFromSct(this.reader, (int)this.header.Version);
                }
            }
        }

        public IEnumerable<Bitmap> EnumerateTilesBitmaps(Color[] palette)
        {
            this.reader.BaseStream.Seek(this.header.PtrTiles, SeekOrigin.Begin);
            for (int i = 0; i < this.header.Tiles; i++)
            {
                byte[] tile = Util.LoadMapTile1D(this.reader);
                yield return Util.GetBitmap(tile, palette);
            }
        }

        public IEnumerable<Bitmap> EnumerateTileDataBitmaps(Color[] palette)
        {
            Bitmap[] tiles = this.GetTilesBitmaps(palette);

            foreach (int i in this.Data)
            {
                yield return tiles[i];
            }
        }

        public byte[][] GetTiles()
        {
            byte[][] tiles = new byte[this.header.Tiles][];
            int i = 0;
            foreach (byte[] t in this.Tiles)
            {
                tiles[i] = t;
                i++;
            }

            return tiles;
        }

        public Bitmap[] GetTilesBitmaps(Color[] palette)
        {
            Bitmap[] tiles = new Bitmap[this.header.Tiles];
            int i = 0;
            foreach (Bitmap t in this.EnumerateTilesBitmaps(palette))
            {
                tiles[i] = t;
                i++;
            }

            return tiles;
        }

        public byte[,][] GetTileData()
        {
            byte[][] tiles = this.GetTiles();

            IEnumerator<int> data = this.Data.GetEnumerator();

            byte[,][] map = new byte[this.Height, this.Width][];

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    data.MoveNext();
                    map[y, x] = tiles[data.Current];
                }
            }

            return map;
        }

        public TileAttr[,] GetAttrs()
        {
            TileAttr[,] a = new TileAttr[this.Height * 2, this.Width * 2];
            IEnumerator<TileAttr> attrs = this.Attrs.GetEnumerator();
            for (int y = 0; y < (this.Height * 2); y++)
            {
                for (int x = 0; x < (this.Width * 2); x++)
                {
                    attrs.MoveNext();
                    a[y, x] = attrs.Current;
                }
            }

            return a;
        }

        public byte[,] GetMinimap()
        {
            byte[,] map = new byte[128, 128];
            this.reader.BaseStream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    map[y, x] = this.reader.ReadByte();
                }
            }

            return map;
        }

        public Bitmap GetMinimapBitmap(Color[] palette)
        {
            this.reader.BaseStream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
            return Util.LoadBitmap(this.reader, palette, 128, 128);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.reader.Dispose();
            }
        }
    }
}
