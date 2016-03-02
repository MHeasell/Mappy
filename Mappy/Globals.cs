namespace Mappy
{
    using System.Drawing;

    public static class Globals
    {
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
