namespace TAUtil.Gdi.Bitmap
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using TAUtil.Gdi.Palette;

    /// <summary>
    /// Serializes a 32-bit Bitmap instance into raw 8-bit indexed color data.
    /// The mapping from color to index is done according to the given
    /// reverse palette lookup.
    /// </summary>
    public class BitmapSerializer
    {
        private readonly IPalette palette;

        public BitmapSerializer(IPalette palette)
        {
            this.palette = palette;
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
            BitmapData data = bitmap.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int length = bitmap.Width * bitmap.Height;

            unsafe
            {
                int* pointer = (int*)data.Scan0;
                for (int i = 0; i < length; i++)
                {
                    Color c = Color.FromArgb(pointer[i]);
                    output.WriteByte((byte)this.palette.LookUp(c));
                }
            }

            bitmap.UnlockBits(data);
        }
    }
}