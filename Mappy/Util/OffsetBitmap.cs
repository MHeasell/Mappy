namespace Mappy.Util
{
    using System.Drawing;

    public class OffsetBitmap
    {
        public OffsetBitmap(int offsetX, int offsetY, Bitmap bitmap)
        {
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.Bitmap = bitmap;
        }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public Bitmap Bitmap { get; set; }
    }
}
