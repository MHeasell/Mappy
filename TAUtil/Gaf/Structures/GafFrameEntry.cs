namespace TAUtil.Gaf.Structures
{
    using System.IO;

    public struct GafFrameEntry
    {
        /// <summary>
        /// Pointer to frame table
        /// </summary>
        public uint PtrFrameTable;

        /// <summary>
        /// Unknown - varies
        /// </summary>
        public uint Unknown1;

        public static void Read(Stream f, ref GafFrameEntry e)
        {
            BinaryReader b = new BinaryReader(f);
            e.PtrFrameTable = b.ReadUInt32();
            e.Unknown1 = b.ReadUInt32();
        }

        public void Write(Stream f)
        {
            BinaryWriter b = new BinaryWriter(f);
            b.Write(this.PtrFrameTable);
            b.Write(this.Unknown1);
        }
    }
}
