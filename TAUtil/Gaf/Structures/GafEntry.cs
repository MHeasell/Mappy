namespace TAUtil.Gaf.Structures
{
    using System.IO;

    public struct GafEntry
    {
        public ushort Frames;
        public ushort Unknown1;
        public uint Unknown2;
        public string Name;

        public static void Read(BinaryReader b, ref GafEntry entry)
        {
            entry.Frames = b.ReadUInt16();
            entry.Unknown1 = b.ReadUInt16();
            entry.Unknown2 = b.ReadUInt32();
            entry.Name = Util.ConvertChars(b.ReadBytes(32));
        }
    }
}
