namespace TAUtil.Sct
{
    using System.IO;

    public struct SctHeader
    {
        /// <summary>
        /// SCT version. Usually set to  2 or 3.
        /// </summary>
        public uint Version;

        /// <summary>
        /// Offset to the section's minimap.
        /// </summary>
        public uint PtrMiniMap;

        /// <summary>
        /// The number of tiles in the section.
        /// </summary>
        public uint Tiles;

        /// <summary>
        /// Offset to the section's tiles.
        /// </summary>
        public uint PtrTiles;

        /// <summary>
        /// Width of the section in image tiles.
        /// For dimensions in terms of attributes, multiply by 2.
        /// </summary>
        public uint Width;

        /// <summary>
        /// Height of the section in image tiles.
        /// For dimensions in terms of attributes, multiply by 2.
        /// </summary>
        public uint Height;

        /// <summary>
        /// Offest to the section's data.
        /// </summary>
        public uint PtrData;

        public uint PtrHeightData
        {
            get
            {
                return this.PtrData + (this.Width * this.Height * sizeof(short));
            }
        }

        public static void Read(BinaryReader b, ref SctHeader header)
        {
            uint version = b.ReadUInt32();
            if (version != 2 && version != 3)
            {
                throw new ParseException("Input does not appear to be a valid SCT file");
            }

            header.Version = version;

            header.PtrMiniMap = b.ReadUInt32();
            header.Tiles = b.ReadUInt32();
            header.PtrTiles = b.ReadUInt32();
            header.Width = b.ReadUInt32();
            header.Height = b.ReadUInt32();
            header.PtrData = b.ReadUInt32();
        }

        public void Write(Stream s)
        {
            this.Write(new BinaryWriter(s));
        }

        public void Write(BinaryWriter b)
        {
            b.Write(this.Version);
            b.Write(this.PtrMiniMap);
            b.Write(this.Tiles);
            b.Write(this.PtrTiles);
            b.Write(this.Width);
            b.Write(this.Height);
            b.Write(this.PtrData);
        }
    }
}
