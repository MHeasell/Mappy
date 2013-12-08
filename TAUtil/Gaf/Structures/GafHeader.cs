namespace TAUtil.Gaf.Structures
{
    using System.IO;

    public struct GafHeader
    {
        /// <summary>
        /// Version stamp - always 0x00010100
        /// </summary>
        public uint IdVersion;

        /// <summary>
        /// Number of items contained in this file
        /// </summary>
        public uint Entries;

        /// <summary>
        /// Always 0
        /// </summary>
        public uint Unknown1;

        public static void Read(BinaryReader b, ref GafHeader header)
        {
            header.IdVersion = b.ReadUInt32();
            header.Entries = b.ReadUInt32();
            header.Unknown1 = b.ReadUInt32();
        }

        public void Write(BinaryWriter b)
        {
            b.Write(this.IdVersion);
            b.Write(this.Entries);
            b.Write(this.Unknown1);
        }
    }
}
