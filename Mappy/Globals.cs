namespace Mappy
{
    using System.Drawing;
    using Mappy.IO;
    using Mappy.Services;

    public static class Globals
    {
        public static readonly BitmapCache TileCache = new BitmapCache();

        public static readonly Bitmap DefaultTile = CreateWhiteBitmap(32, 32);

        private static Bitmap CreateWhiteBitmap(int width, int height)
        {
            var bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            }

            return bitmap;
        }
    }
}
