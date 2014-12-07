namespace TAUtil.Gdi.Bitmap
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using TAUtil.Gdi.Palette;

    /// <summary>
    /// Deserializes raw 8-bit color image data into a 32-bit Bitmap instance.
    /// The colors are translated according to the given palette.
    /// </summary>
    public class BitmapDeserializer
    {
        private readonly IPalette palette;

        public BitmapDeserializer(IPalette palette)
        {
            this.palette = palette;
        }

        public Bitmap Deserialize(byte[] bytes, int width, int height)
        {
            return this.Deserialize(new MemoryStream(bytes), width, height);
        }

        public Bitmap Deserialize(Stream bytes, int width, int height)
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
                    int readByte = bytes.ReadByte();
                    pointer[i] = this.palette[readByte].ToArgb();
                }
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }
    }
}
