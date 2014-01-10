namespace TAUtil.Sct
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using TAUtil.Tnt;

    public class SctReader : IDisposable, ISctSource
    {
        public const int MinimapWidth = 128;

        public const int MinimapHeight = 128;

        private readonly BinaryReader reader;

        private SctHeader header;

        public SctReader(string filename)
            : this(File.OpenRead(filename))
        {
        }

        public SctReader(Stream s)
            : this(new BinaryReader(s))
        {
        }

        public SctReader(BinaryReader r)
        {
            this.reader = r;
            SctHeader.Read(this.reader, ref this.header);
        }

        public int DataWidth
        {
            get { return (int)this.header.Width; }
        }

        public int DataHeight
        {
            get { return (int)this.header.Height; }
        }

        public int WidthInAttrs
        {
            get
            {
                return (int)this.header.Width * 2;
            }
        }

        public int HeightInAttrs
        {
            get
            {
                return (int)this.header.Height * 2;
            }
        }

        public int TileCount
        {
            get
            {
                return (int)this.header.Tiles;
            }
        }

        public byte[] GetMinimap()
        {
            this.reader.BaseStream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
            return this.reader.ReadBytes(MinimapWidth * MinimapHeight);
        }

        public IEnumerable<TileAttr> EnumerateAttrs()
        {
            this.reader.BaseStream.Seek(this.header.PtrHeightData, SeekOrigin.Begin);
            for (int y = 0; y < this.HeightInAttrs; y++)
            {
                for (int x = 0; x < this.WidthInAttrs; x++)
                {
                    yield return TileAttr.ReadFromSct(this.reader, (int)this.header.Version);
                }
            }
        }

        public IEnumerable<int> EnumerateData() 
        {
            this.reader.BaseStream.Seek(this.header.PtrData, SeekOrigin.Begin);
            for (int y = 0; y < this.DataHeight; y++)
            {
                for (int x = 0; x < this.DataWidth; x++)
                {
                    yield return this.reader.ReadInt16();
                }
            }
        }

        public IEnumerable<byte[]> EnumerateTiles()
        {
            this.reader.BaseStream.Seek(this.header.PtrTiles, SeekOrigin.Begin);
            for (int i = 0; i < this.TileCount; i++)
            {
                yield return this.reader.ReadBytes(MapConstants.TileDataLength);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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
