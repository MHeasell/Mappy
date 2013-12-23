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

            if (width > 252 || height > 252)
            {
                throw new ArgumentException("dimensions exceed maximum minimap bounds");
            }

            if (data.Length != width * height)
            {
                throw new ArgumentException("data length does not match given dimensions");
            }

            this.writer.Write(252); // width
            this.writer.Write(252); // height

            for (int y = 0; y < 252; y++)
            {
                for (int x = 0; x < 252; x++)
                {
                    if (x >= width || y >= height)
                    {
                        this.writer.Write(TntReader.MinimapVoidByte);
                        continue;
                    }

                    this.writer.Write(data[(y * width) + x]);
                }
            }
        }

        public void WriteTile(byte[] data)
        {
            if (data.Length != 32 * 32)
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
            if (name.Length >= 127)
            {
                throw new ArgumentException("name is too long");
            }

            if (index < 0)
            {
                throw new ArgumentException("index must not be negative");
            }

            byte[] c = new byte[128];
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
