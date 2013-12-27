namespace Mappy.IO
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class BitmapDeserializer
    {
        private readonly Color[] palette;

        public BitmapDeserializer(Color[] palette)
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
                    pointer[i] = this.palette[bytes.ReadByte()].ToArgb();
                }
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }
    }
}
