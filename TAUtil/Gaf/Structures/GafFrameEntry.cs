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

        public static void Read(BinaryReader b, ref GafFrameEntry e)
        {
            e.PtrFrameTable = b.ReadUInt32();
            e.Unknown1 = b.ReadUInt32();
        }

        public void Write(BinaryWriter b)
        {
            b.Write(this.PtrFrameTable);
            b.Write(this.Unknown1);
        }
    }
}
