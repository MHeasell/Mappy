namespace Mappy.IO
{
    using System.Drawing;

    public class BitmapDeserializer : AbstractBitmapDeserializer
    {
        private readonly Color[] palette;

        public BitmapDeserializer(Color[] palette)
        {
            this.palette = palette;
        }

        protected override Color ToColor(int index)
        {
            return this.palette[index];
        }
    }
}
