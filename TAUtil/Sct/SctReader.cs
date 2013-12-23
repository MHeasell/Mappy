namespace TAUtil.Sct
{
    using System;
    using System.IO;

    using TAUtil.Tnt;

    public class SctReader : IDisposable
    {
        public const int MinimapWidth = 128;

        public const int MinimapHeight = 128;

        public const int TileWidth = 32;

        public const int TileHeight = 32;

        private readonly BinaryReader reader;

        private SctHeader header;

        public SctReader(Stream s)
        {
            this.reader = new BinaryReader(s);
            SctHeader.Read(this.reader, ref this.header);
        }

        ~SctReader()
        {
            this.Dispose(false);
        }

        public int Width
        {
            get { return (int)this.header.Width; }
        }

        public int Height
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

        public void SeekToTiles()
        {
            this.reader.BaseStream.Seek(this.header.PtrTiles, SeekOrigin.Begin);
        }

        public void SeekToData()
        {
            this.reader.BaseStream.Seek(this.header.PtrData, SeekOrigin.Begin);
        }

        public void SeekToMinimap()
        {
            this.reader.BaseStream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
        }

        public void SeekToAttrs()
        {
            this.reader.BaseStream.Seek(this.header.PtrHeightData, SeekOrigin.Begin);
        }

        public byte[] ReadMinimap()
        {
            this.reader.BaseStream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
            return this.reader.ReadBytes(MinimapWidth * MinimapHeight);
        }

        public byte[] ReadTile()
        {
            return this.reader.ReadBytes(SctReader.TileWidth * SctReader.TileHeight);
        }

        public byte ReadPixel()
        {
            return this.reader.ReadByte();
        }

        public short ReadDataCell()
        {
            return this.reader.ReadInt16();
        }

        public TileAttr ReadAttr()
        {
            return TileAttr.ReadFromSct(this.reader, (int)this.header.Version);
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
