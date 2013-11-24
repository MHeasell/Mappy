namespace TAUtil.Tnt
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    public class TntWriter
    {
        public TntWriter(Stream s)
        {
            this.BaseStream = s;
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
                        w.Write(TntFile.MinimapVoidByte);
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
            foreach (Bitmap tile in tiles)
            {
                this.WriteTile(tile, palette);
            }
        }

        public void WriteTile(Bitmap tile, Color[] palette)
        {
            for (int y = 0; y < tile.Height; y++)
            {
                for (int x = 0; x < tile.Width; x++)
                {
                    Color c = tile.GetPixel(x, y);
                    int i = Array.IndexOf(palette, c);
                    this.BaseStream.WriteByte((byte)i);
                }
            }
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
            BinaryWriter w = new BinaryWriter(this.BaseStream);
            foreach (Bitmap b in data)
            {
                int i = Array.IndexOf(mapping, b);
                w.Write((short)i);
            }
        }
    }
}
