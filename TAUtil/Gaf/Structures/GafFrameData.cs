namespace TAUtil.Gaf.Structures
{
    using System.IO;

    public struct GafFrameData
    {
        public ushort Width;
        public ushort Height;
        public ushort XPos;
        public ushort YPos;
        public byte Unknown1;
        public bool Compressed;
        public ushort FramePointers;
        public uint Unknown2;
        public uint PtrFrameData;
        public uint Unknown3;

        public static void Read(BinaryReader b, ref GafFrameData e)
        {
            e.Width = b.ReadUInt16();
            e.Height = b.ReadUInt16();
            e.XPos = b.ReadUInt16();
            e.YPos = b.ReadUInt16();
            e.Unknown1 = b.ReadByte();
            e.Compressed = b.ReadBoolean();
            e.FramePointers = b.ReadUInt16();
            e.Unknown2 = b.ReadUInt32();
            e.PtrFrameData = b.ReadUInt32();
            e.Unknown3 = b.ReadUInt32();
        }
    }
}
