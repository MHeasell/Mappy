namespace TAUtil.Tnt
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class TntReader : IDisposable, ITntSource
    {
        private readonly BinaryReader reader;

        private TntHeader header;

        public TntReader(Stream s)
        {
            this.reader = new BinaryReader(s);
            TntHeader.Read(this.reader, ref this.header);
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

        public int TileCount
        {
            get
            {
                return (int)this.header.Tiles;
            }
        }

        public int AnimCount
        {
            get
            {
                return (int)this.header.TileAnims;
            }
        }

        public IEnumerable<TileAttr> EnumerateAttrs()
        {
            this.reader.BaseStream.Seek(this.header.PtrMapAttr, SeekOrigin.Begin);
            for (int y = 0; y < this.header.Height; y++)
            {
                for (int x = 0; x < this.header.Width; x++)
                {
                    yield return TileAttr.Read(this.reader);
                }
            }
        }

        public IEnumerable<int> EnumerateData()
        {
            this.reader.BaseStream.Seek(this.header.PtrMapData, SeekOrigin.Begin);
            int length = this.DataWidth * this.DataHeight;
            for (int i = 0; i < length; i++)
            {
                yield return this.reader.ReadInt16();
            }
        }

        public IEnumerable<byte[]> EnumerateTiles()
        {
            this.reader.BaseStream.Seek(this.header.PtrTileGfx, SeekOrigin.Begin);
            for (int i = 0; i < this.TileCount; i++)
            {
                yield return this.reader.ReadBytes(MapConstants.TileDataLength);
            }
        }

        public IEnumerable<string> EnumerateAnims()
        {
            this.reader.BaseStream.Seek(this.header.PtrTileAnims, SeekOrigin.Begin);
            for (int i = 0; i < this.AnimCount; i++)
            {
                this.reader.ReadUInt32(); // skip feature index
                byte[] chars = this.reader.ReadBytes(TntConstants.AnimNameLength);
                string s = TAUtil.Util.ConvertChars(chars);
                yield return s;
            }
        }

        public MinimapInfo GetMinimap()
        {
            this.reader.BaseStream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
            int width = this.reader.ReadInt32();
            int height = this.reader.ReadInt32();
            byte[] data = this.reader.ReadBytes(width * height);
            Util.Size trimmedSize = Util.GetMinimapActualSize(data, width, height);
            data = Util.TrimMinimapBytes(
                data,
                width,
                height,
                trimmedSize.Width,
                trimmedSize.Height);
            MinimapInfo minimap = new MinimapInfo(trimmedSize.Width, trimmedSize.Height, data);

            return minimap;
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
