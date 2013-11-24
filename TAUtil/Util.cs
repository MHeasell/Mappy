namespace TAUtil
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class Util
    {
        public const int TileSize = 32;

        public static readonly IDictionary<string, Bitmap> ImageMap = new Dictionary<string, Bitmap>();

        private static readonly SHA1 Sha = new SHA1CryptoServiceProvider();

        public static string HashTile(byte[] tile)
        {
            return Encoding.ASCII.GetString(Util.Sha.ComputeHash(tile));
        }

        public static Bitmap GetBitmap(byte[] tile, Color[] palette)
        {
            string hash = Util.HashTile(tile);
            Bitmap b;
            if (!Util.ImageMap.TryGetValue(hash, out b))
            {
                b = Util.ConvertToBitmap(tile, palette, 32, 32);
                Util.ImageMap[hash] = b;
            }

            return b;
        }

        public static string ConvertChars(byte[] data)
        {
            int i = Array.IndexOf<byte>(data, 0);
            Debug.Assert(i != -1, "null terminator not found");
            return System.Text.Encoding.ASCII.GetString(data, 0, i);
        }

        public static byte[,] LoadMapTile(Stream f)
        {
            BinaryReader b = new BinaryReader(f);
            byte[,] t = new byte[TileSize, TileSize];

            for (int y = 0; y < TileSize; y++)
            {
                for (int x = 0; x < TileSize; x++)
                {
                    t[y, x] = b.ReadByte();
                }
            }

            return t;
        }

        public static byte[] LoadMapTile1D(Stream f)
        {
            BinaryReader b = new BinaryReader(f);
            return b.ReadBytes(TileSize * TileSize);
        }

        public static Bitmap LoadMapTileBitmap(Stream f, Color[] p)
        {
            return LoadBitmap(f, p, TileSize, TileSize);
        }

        public static Bitmap LoadBitmap(Stream f, Color[] p, int width, int height)
        {
            BinaryReader b = new BinaryReader(f);

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(new Point(0, 0), bitmap.Size);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

            unsafe
            {
                byte* pointer = (byte*)data.Scan0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color c = p[b.ReadByte()];
                        int pos = (y * width * 4) + (x * 4);
                        pointer[pos] = c.B;
                        pointer[pos + 1] = c.G;
                        pointer[pos + 2] = c.R;
                        pointer[pos + 3] = c.A;
                    }
                }
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }

        public static Bitmap ConvertToBitmap(byte[] arr, Color[] p, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(new Point(0, 0), bitmap.Size);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

            unsafe
            {
                byte* pointer = (byte*)data.Scan0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color c = p[arr[(y * width) + x]];
                        int pos = (y * width * 4) + (x * 4);
                        pointer[pos] = c.B;
                        pointer[pos + 1] = c.G;
                        pointer[pos + 2] = c.R;
                        pointer[pos + 3] = c.A;
                    }
                }
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }
    }
}
