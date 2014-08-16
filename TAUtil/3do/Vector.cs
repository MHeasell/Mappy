namespace TAUtil._3do
{
    using System.IO;

    public struct Vector
    {
        public int X;

        public int Y;

        public int Z;

        public static void Read(Stream s, ref Vector v)
        {
            Read(new BinaryReader(s), ref v);
        }

        public static void Read(BinaryReader b, ref Vector v)
        {
            v.X = b.ReadInt32();
            v.Y = b.ReadInt32();
            v.Z = b.ReadInt32();
        }
    }
}
