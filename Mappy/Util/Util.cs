namespace Mappy.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Security.Cryptography;
    using System.Text;

    using Data;
    using Geometry;
    using Grids;

    using TAUtil.Tnt;

    public static class Util
    {
        private static readonly IDictionary<string, Bitmap> ImageMap = new Dictionary<string, Bitmap>();

        private static readonly SHA1 Sha = SHA1.Create();

        public static HashSet<Bitmap> GetUsedTiles(IMapTile tile)
        {
            HashSet<Bitmap> set = new HashSet<Bitmap>();
            foreach (Bitmap b in tile.TileGrid)
            {
                set.Add(b);
            }

            return set;
        }

        public static Point? ScreenToHeightIndex(IGrid<int> heightmap, Point p)
        {
            int col = p.X / 16;

            if (col < 0 || col >= heightmap.Height)
            {
                return null;
            }

            Ray3D ray = new Ray3D(new Vector3D(p.X, p.Y + 128, 255.0), new Vector3D(0.0, -0.5, -1.0));

            for (int row = heightmap.Height - 1; row >= 0; row--)
            {
                int height = heightmap.Get(col, row);
                AxisRectangle3D rect = new AxisRectangle3D(
                    new Vector3D((col * 16) + 0.5, (row * 16) + 0.5, height),
                    16.0,
                    16.0);
                double distance;
                if (rect.Intersect(ray, out distance))
                {
                    return new Point(col, row);
                }
            }

            return null;
        }

        public static double Mod(double a, double p)
        {
            return ((a % p) + p) % p;
        }

        public static int Mod(int a, int p)
        {
            return ((a % p) + p) % p;
        }

        public static Bitmap GetStartImage(int index)
        {
            switch (index)
            {
                case 1:
                    return Mappy.Properties.Resources.number_1;
                case 2:
                    return Mappy.Properties.Resources.number_2;
                case 3:
                    return Mappy.Properties.Resources.number_3;
                case 4:
                    return Mappy.Properties.Resources.number_4;
                case 5:
                    return Mappy.Properties.Resources.number_5;
                case 6:
                    return Mappy.Properties.Resources.number_6;
                case 7:
                    return Mappy.Properties.Resources.number_7;
                case 8:
                    return Mappy.Properties.Resources.number_8;
                case 9:
                    return Mappy.Properties.Resources.number_9;
                case 10:
                    return Mappy.Properties.Resources.number_10;
                default:
                    throw new ArgumentException("invalid index: " + index);
            }
        }

        public static Bitmap ReadToBitmap(
            byte[] bytes,
            Color[] palette,
            int width,
            int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(new Point(0, 0), bitmap.Size);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

            int length = width * height;

            unsafe
            {
                int* pointer = (int*)data.Scan0;
                for (int i = 0; i < length; i++)
                {
                    pointer[i] = palette[bytes[i]].ToArgb();
                }
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }

        public static Bitmap AddTileToDatabase(byte[] tileData, Color[] palette)
        {
            string hash = Encoding.ASCII.GetString(Sha.ComputeHash(tileData));
            Bitmap bmp;
            if (!ImageMap.TryGetValue(hash, out bmp))
            {
                bmp = ReadToBitmap(tileData, palette, TntReader.TileWidth, TntReader.TileHeight);
                ImageMap[hash] = bmp;
            }

            return bmp;
        }
    }
}
