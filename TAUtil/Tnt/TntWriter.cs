namespace TAUtil.Tnt
{
    using System;
    using System.IO;

    public class TntWriter : IDisposable
    {
        private readonly BinaryWriter writer;

        public TntWriter(Stream s)
        {
            this.BaseStream = s;
            this.writer = new BinaryWriter(s);
        }

        ~TntWriter()
        {
            this.Dispose(false);
        }

        public Stream BaseStream { get; private set; }

        public void WriteMinimap(int width, int height, byte[] data)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("dimensions are invalid");
            }

            if (width > TntConstants.MaxMinimapWidth || height > TntConstants.MaxMinimapHeight)
            {
                throw new ArgumentException("dimensions exceed maximum minimap bounds");
            }

            if (data.Length != width * height)
            {
                throw new ArgumentException("data length does not match given dimensions");
            }

            this.writer.Write(TntConstants.MaxMinimapWidth);
            this.writer.Write(TntConstants.MaxMinimapHeight);

            for (int y = 0; y < TntConstants.MaxMinimapHeight; y++)
            {
                for (int x = 0; x < TntConstants.MaxMinimapWidth; x++)
                {
                    if (x >= width || y >= height)
                    {
                        this.writer.Write(TntConstants.MinimapVoidByte);
                        continue;
                    }

                    this.writer.Write(data[(y * width) + x]);
                }
            }
        }

        public void WriteTile(byte[] data)
        {
            if (data.Length != MapConstants.TileDataLength)
            {
                throw new ArgumentException("data is not tile sized");
            }

            this.writer.Write(data);
        }

        public void WritePixel(byte pixel)
        {
            this.writer.Write(pixel);
        }

        public void WriteAttr(TileAttr attr)
        {
            attr.Write(this.writer);
        }

        public void WriteAnim(string name)
        {
            // TA seems to ignore index field,
            // so setting it to 0 is okay.
            this.WriteAnim(name, 0);
        }

        public void WriteAnim(string name, int index)
        {
            if (name.Length >= TntConstants.AnimNameLength - 1)
            {
                throw new ArgumentException("name is too long");
            }

            if (index < 0)
            {
                throw new ArgumentException("index must not be negative");
            }

            byte[] c = new byte[TntConstants.AnimNameLength];
            System.Text.Encoding.ASCII.GetBytes(name, 0, name.Length, c, 0);
            this.writer.Write(index);
            this.writer.Write(c);
        }

        public void WriteDataCell(short data)
        {
            BinaryWriter b = new BinaryWriter(this.BaseStream);
            b.Write(data);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.BaseStream.Dispose();
            }
        }
    }
}
