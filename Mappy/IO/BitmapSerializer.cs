namespace Mappy.IO
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using Mappy.Palette;

    public class BitmapSerializer
    {
        private readonly IReversePalette reversePalette;

        public BitmapSerializer(IReversePalette reversePalette)
        {
            this.reversePalette = reversePalette;
        }

        public byte[] ToBytes(Bitmap bitmap)
        {
            int length = bitmap.Width * bitmap.Height;

            byte[] output = new byte[length];

            this.Serialize(new MemoryStream(output, true), bitmap);

            return output;
        }

        public void Serialize(Stream output, Bitmap bitmap)
        {
            Rectangle r = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(r, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            int length = bitmap.Width * bitmap.Height;

            unsafe
            {
                int* pointer = (int*)data.Scan0;
                for (int i = 0; i < length; i++)
                {
                    Color c = Color.FromArgb(pointer[i]);
                    output.WriteByte((byte)this.reversePalette[c]);
                }
            }

            bitmap.UnlockBits(data);
        }
    }
}