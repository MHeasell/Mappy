namespace Mappy.IO
{
    using System.Drawing;

    public class KeyedBitmapDeserializer : AbstractBitmapDeserializer
    {
        private readonly Color[] palette;

        public KeyedBitmapDeserializer(Color[] palette)
        {
            this.palette = palette;
        }

        public byte TransparencyKey { get; set; }

        protected override Color ToColor(int index)
        {
            return index == this.TransparencyKey ? Color.Transparent : this.palette[index];
        }
    }
}
