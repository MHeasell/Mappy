namespace TAUtil.Gdi.Bitmap
{
    using System.Drawing;
    using System.IO;

    using TAUtil.Gdi.Palette;

    public static class BitmapConvert
    {
        private static readonly BitmapSerializer Serializer;

        private static readonly BitmapDeserializer Deserializer;

        static BitmapConvert()
        {
            Serializer = new BitmapSerializer(PaletteFactory.TAPalette);
            Deserializer = new BitmapDeserializer(PaletteFactory.TAPalette);
        }

        public static Bitmap ToBitmap(Stream bytes, int width, int height)
        {
            return Deserializer.Deserialize(bytes, width, height);
        }

        public static Bitmap ToBitmap(byte[] bytes, int width, int height)
        {
            return Deserializer.Deserialize(bytes, width, height);
        }

        public static Bitmap ToBitmap(Stream bytes, int width, int height, int transparencyIndex)
        {
            var transPalette = new TransparencyMaskedPalette(PaletteFactory.TAPalette, transparencyIndex);
            var des = new BitmapDeserializer(transPalette);
            return des.Deserialize(bytes, width, height);
        }

        public static Bitmap ToBitmap(byte[] bytes, int width, int height, int transparencyIndex)
        {
            var transPalette = new TransparencyMaskedPalette(PaletteFactory.TAPalette, transparencyIndex);
            var des = new BitmapDeserializer(transPalette);
            return des.Deserialize(bytes, width, height);
        }

        public static byte[] ToBytes(Bitmap bitmap)
        {
            return Serializer.ToBytes(bitmap);
        }

        public static byte[] ToBytes(Bitmap bitmap, int transparencyIndex)
        {
            var transPalette = new TransparencyMaskedPalette(PaletteFactory.TAPalette, transparencyIndex);
            var ser = new BitmapSerializer(transPalette);
            return ser.ToBytes(bitmap);
        }

        public static void ToStream(Stream s, Bitmap bitmap)
        {
            Serializer.Serialize(s, bitmap);
        }

        public static void ToStream(Stream s, Bitmap bitmap, int transparencyIndex)
        {
            var transPalette = new TransparencyMaskedPalette(PaletteFactory.TAPalette, transparencyIndex);
            var ser = new BitmapSerializer(transPalette);
            ser.Serialize(s, bitmap);
        }
    }
}
