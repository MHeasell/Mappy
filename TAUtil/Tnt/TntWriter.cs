namespace TAUtil.Tnt
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class TntWriter : IDisposable
    {
        public TntWriter(Stream s)
        {
            this.BaseStream = s;
        }

        ~TntWriter()
        {
            this.Dispose(false);
        }

        public Stream BaseStream { get; private set; }

        public void WriteMinimap(Bitmap minimap, Color[] palette)
        {
            BinaryWriter w = new BinaryWriter(this.BaseStream);

            w.Write(252); // width
            w.Write(252); // height

            for (int y = 0; y < 252; y++)
            {
                for (int x = 0; x < 252; x++)
                {
                    if (x >= minimap.Width || y >= minimap.Height)
                    {
                        w.Write(TntReader.MinimapVoidByte);
                        continue;
                    }

                    Color c = minimap.GetPixel(x, y);
                    int i = Array.IndexOf(palette, c);
                    this.BaseStream.WriteByte((byte)i);
                }
            }
        }

        public void WriteTiles(IEnumerable<Bitmap> tiles, Color[] palette)
        {
            var d = new Dictionary<Color, int>();
            for (int i = 0; i < palette.Length; i++)
            {
                d[palette[i]] = i;
            }

            foreach (Bitmap tile in tiles)
            {
                this.WriteTile(tile, d);
            }
        }

        public void WriteTile(Bitmap tile, IDictionary<Color, int> palette)
        {
            Rectangle r = new Rectangle(0, 0, tile.Width, tile.Height);
            BitmapData data = tile.LockBits(r, ImageLockMode.ReadOnly, tile.PixelFormat);

            unsafe
            {
                int* pointer = (int*)data.Scan0;
                int count = tile.Width * tile.Height;
                for (int i = 0; i < count; i++)
                {
                    Color c = Color.FromArgb(pointer[i]);
                    int colorIndex = palette[c];
                    this.BaseStream.WriteByte((byte)colorIndex);
                }
            }

            tile.UnlockBits(data);
        }

        public void WriteAttrs(IEnumerable<TileAttr> attrs)
        {
            foreach (TileAttr attr in attrs)
            {
                attr.Write(this.BaseStream);
            }
        }

        public void WriteAnimNames(IEnumerable<string> names)
        {
            int i = 0;
            foreach (string name in names)
            {
                this.WriteAnimName(name, i++);
            }
        }

        public void WriteAnimName(string name)
        {
            // TA seems to ignore index field,
            // so setting it to 0 is okay.
            this.WriteAnimName(name, 0);
        }

        public void WriteAnimName(string name, int index)
        {
            BinaryWriter b = new BinaryWriter(this.BaseStream);
            byte[] c = new byte[128];
            System.Text.Encoding.ASCII.GetBytes(name, 0, name.Length, c, 0);
            c[name.Length] = 0; // make sure null terminator is set
            b.Write(index);
            b.Write(c);
        }

        public void WriteMapData(IEnumerable<int> data)
        {
            BinaryWriter b = new BinaryWriter(this.BaseStream);
            foreach (int i in data)
            {
                b.Write((short)i);
            }
        }

        public void WriteMapData(IEnumerable<Bitmap> data, Bitmap[] mapping)
        {
            var d = new Dictionary<Bitmap, int>();
            for (int i = 0; i < mapping.Length; i++)
            {
                d[mapping[i]] = i;
            }

            BinaryWriter w = new BinaryWriter(this.BaseStream);
            foreach (Bitmap b in data)
            {
                w.Write((short)d[b]);
            }
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
