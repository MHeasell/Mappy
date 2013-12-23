namespace TAUtil.Tnt
{
    using System;
    using System.IO;

    public class TntReader : IDisposable
    {
        private readonly BinaryReader reader;

        private TntHeader header;

        public TntReader(Stream s)
        {
            this.reader = new BinaryReader(s);
            TntHeader.Read(this.reader, ref this.header);
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

        public static MinimapInfo TrimMinimapBytes(MinimapInfo minimap)
        {
            var realSize = TntReader.GetMinimapActualSize(minimap);

            MinimapInfo newMinimap = new MinimapInfo(realSize.Item1, realSize.Item2);

            for (int y = 0; y < newMinimap.Height; y++)
            {
                for (int x = 0; x < newMinimap.Width; x++)
                {
                    newMinimap.Data[(y * newMinimap.Width) + x] = minimap.Data[(y * minimap.Width) + x];
                }
            }

            return newMinimap;
        }

        public void SeekToTiles()
        {
            this.reader.BaseStream.Seek(this.header.PtrTileGfx, SeekOrigin.Begin);
        }

        public void SeekToAnims()
        {
            this.reader.BaseStream.Seek(this.header.PtrTileAnims, SeekOrigin.Begin);
        }

        public void SeekToData()
        {
            this.reader.BaseStream.Seek(this.header.PtrMapData, SeekOrigin.Begin);
        }

        public void SeekToMinimap()
        {
            this.reader.BaseStream.Seek(this.header.PtrMiniMap, SeekOrigin.Begin);
        }

        public void SeekToAttrs()
        {
            this.reader.BaseStream.Seek(this.header.PtrMapAttr, SeekOrigin.Begin);
        }

        public byte ReadPixel()
        {
            return this.reader.ReadByte();
        }

        public short ReadDataCell()
        {
            return this.reader.ReadInt16();
        }

        public string ReadAnim()
        {
            this.reader.ReadUInt32();
            byte[] chars = this.reader.ReadBytes(128);
            string s = Util.ConvertChars(chars);
            return s;
        }

        public TileAttr ReadAttr()
        {
            return TileAttr.Read(this.reader);
        }

        public byte[] ReadTile()
        {
            return this.reader.ReadBytes(MapConstants.TileDataLength);
        }

        public MinimapInfo ReadMinimap()
        {
            int readWidth = this.reader.ReadInt32();
            int readHeight = this.reader.ReadInt32();

            MinimapInfo map = new MinimapInfo(readWidth, readHeight);

            this.reader.Read(map.Data, 0, readWidth * readHeight);

            return TrimMinimapBytes(map);
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

        private static Tuple<int, int> GetMinimapActualSize(MinimapInfo minimap)
        {
            int realHeight = 0;
            int realWidth = 0;

            for (int y = 0; y < minimap.Height; y++)
            {
                if (minimap.Data[y * minimap.Width] == TntConstants.MinimapVoidByte)
                {
                    break;
                }

                realHeight++;
            }

            for (int x = 0; x < minimap.Width; x++)
            {
                if (minimap.Data[x] == TntConstants.MinimapVoidByte)
                {
                    break;
                }

                realWidth++;
            }

            return Tuple.Create(realWidth, realHeight);
        }

        public struct MinimapInfo
        {
            public MinimapInfo(int width, int height, byte[] data)
                : this()
            {
                this.Width = width;
                this.Height = height;
                this.Data = data;
            }

            public MinimapInfo(int width, int height)
                : this(width, height, new byte[width * height])
            {
            }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public byte[] Data { get; private set; }
        }
    }
}
