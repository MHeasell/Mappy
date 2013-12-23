namespace TAUtil.Tnt
{
    using System.IO;

    public struct TileAttr
    {
        public const ushort FeatureNone = 0xFFFF;
        public const ushort FeatureVoid = 0xFFFC;
        public const ushort FeatureUnknown = 0xFFFE;

        public byte Height;

        /// <summary>
        /// Offset in the feature array
        /// of the feature located in this square.
        /// 
        /// 0xFFFF if none,
        /// 0xFFFC (-4) if void.
        /// I've also seen 0xFFFE (-2) on some of the early Cavedog maps
        /// such as Lava Run and AC02,
        /// but have no idea what it means.
        /// Please contact me if you have any information!
        /// </summary>
        public ushort Feature;

        /// <summary>
        /// No known purpose
        /// (my personal guess is that it is padding to get to 4 bytes).
        /// </summary>
        public byte Pad1;

        public static TileAttr Read(BinaryReader b)
        {
            TileAttr attr;
            attr.Height = b.ReadByte();
            attr.Feature = b.ReadUInt16();
            attr.Pad1 = b.ReadByte();
            return attr;
        }

        public static TileAttr ReadFromSct(Stream file, int version)
        {
            return ReadFromSct(new BinaryReader(file), version);
        }

        /// <param name="reader"></param>
        /// <param name="version">The SCT format version. Valid versions are 2 and 3.</param>
        /// <returns></returns>
        public static TileAttr ReadFromSct(BinaryReader reader, int version)
        {
            TileAttr a;
            a.Height = reader.ReadByte();
            reader.ReadInt16(); // padding
            reader.ReadByte(); // more padding

            if (version == 2)
            {
                // skip some more padding present in V2
                reader.ReadInt32();
            }

            a.Feature = 0;
            a.Pad1 = 0;
            return a;
        }

        public void Write(BinaryWriter b)
        {
            b.Write(this.Height);
            b.Write(this.Feature);
            b.Write(this.Pad1);
        }
    }
}
