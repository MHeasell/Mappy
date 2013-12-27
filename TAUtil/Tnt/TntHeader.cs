namespace TAUtil.Tnt
{
    using System.IO;

    public struct TntHeader
    {
        public const uint TntMagicNumber = 0x2000;
        public const int HeaderLength = 0x40;

        /// <summary>
        /// This is a version/file type marker of some kind.
        /// It is always 0x2000 in valid TNT files.
        /// </summary>
        public uint IdVersion;

        /// <summary>
        /// The width of the map in 16-pixel units as used for feature/height information.
        /// To get the width in 32-pixel units (the size of a tile), divide by 2.
        /// </summary>
        public uint Width;

        /// <summary>
        /// The height of the map in 16-pixel units as used for feature/height information.
        /// To get the height in 32-pixel units (the size of a tile), divide by 2.
        /// </summary>
        public uint Height;

        /// <summary>
        /// Offest from the beginning of the file to the tile indicies array.
        /// This array contains unsigned shorts
        /// that are the indicies in the tile array
        /// of the 32x32 pixel tile to be displayed at that point.
        /// </summary>
        public uint PtrMapData;

        /// <summary>
        /// Offset from the beginning of the file to the map attributes array.
        /// This array holds the height and feature information
        /// for every 16x16 pixel square on the map.
        /// 
        /// NB: There are 4 of these squares for each square
        /// in the tile indicies array.
        /// </summary>
        public uint PtrMapAttr;

        /// <summary>
        /// This is the array of tiles referenced by the tile indicies array.
        /// One tile consists of unsigned char[32*32].
        /// </summary>
        public uint PtrTileGfx;

        /// <summary>
        /// The length of the array of tiles, i.e. the number of tiles.
        /// </summary>
        public uint Tiles;

        /// <summary>
        /// The length of the array of tile anims (features).
        /// i.e. the number of features.
        /// </summary>
        public uint TileAnims;

        /// <summary>
        /// This is the array of features referenced by the feature indicies array.
        /// Each entry is 0x84 bytes long and consists of an unsigned long
        /// that always seems to equal the index of the feature in this array
        /// and a 128-character null-terminated string
        /// that contains the name of the feature to be placed in the location
        /// as found between the square brackets "[]" in a TDF feature file.
        /// 
        /// NB: The index seems to be totally useless.
        /// I have noticed that while Cavedog maps include them,
        /// Annihilator doesn't even bother to include them.
        /// </summary>
        public uint PtrTileAnims;

        /// <summary>
        /// Any value in the heights array less than this value
        /// is deemed to be "under water"
        /// (i.e. water modifiers and effects apply).
        /// </summary>
        public uint SeaLevel;

        /// <summary>
        /// Obviously enough, this is where the minimap image is found.
        /// At the offset, the first things you find are 2 unsigned long's:
        /// the minimap's width and height (which always seem to be 252).
        /// What follows is width*height of pixel data.
        /// The difficult part about minimaps
        /// is that they are padded by 0xDD's
        /// (the standard blue used as transparent in TA).
        /// </summary>
        public uint PtrMiniMap;

        /// <summary>
        /// No-one knows what this is used for.
        /// Maybe it has to do with multiple-of-4 or power-of-2 sizes again?
        /// </summary>
        public uint Unknown1;

        /// <summary>
        /// No-one knows what this is used for.
        /// Maybe it has to do with multiple-of-4 or power-of-2 sizes again?
        /// </summary>
        public uint Pad1;

        /// <summary>
        /// No-one knows what this is used for.
        /// Maybe it has to do with multiple-of-4 or power-of-2 sizes again?
        /// </summary>
        public uint Pad2;

        /// <summary>
        /// No-one knows what this is used for.
        /// Maybe it has to do with multiple-of-4 or power-of-2 sizes again?
        /// </summary>
        public uint Pad3;

        /// <summary>
        /// No-one knows what this is used for.
        /// Maybe it has to do with multiple-of-4 or power-of-2 sizes again?
        /// </summary>
        public uint Pad4;

        public static void Read(Stream file, ref TntHeader header)
        {
            Read(new BinaryReader(file), ref header);
        }

        public static void Read(BinaryReader b, ref TntHeader header)
        {
            uint version = b.ReadUInt32();
            if (version != TntHeader.TntMagicNumber)
            {
                throw new ParseException("Input does not appear to be a valid TNT file");
            }

            header.IdVersion = version;

            header.Width = b.ReadUInt32();
            header.Height = b.ReadUInt32();
            header.PtrMapData = b.ReadUInt32();
            header.PtrMapAttr = b.ReadUInt32();
            header.PtrTileGfx = b.ReadUInt32();
            header.Tiles = b.ReadUInt32();
            header.TileAnims = b.ReadUInt32();
            header.PtrTileAnims = b.ReadUInt32();
            header.SeaLevel = b.ReadUInt32();
            header.PtrMiniMap = b.ReadUInt32();
            header.Unknown1 = b.ReadUInt32();
            header.Pad1 = b.ReadUInt32();
            header.Pad2 = b.ReadUInt32();
            header.Pad3 = b.ReadUInt32();
            header.Pad4 = b.ReadUInt32();
        }

        public void Write(BinaryWriter b)
        {
            b.Write(this.IdVersion);
            b.Write(this.Width);
            b.Write(this.Height);
            b.Write(this.PtrMapData);
            b.Write(this.PtrMapAttr);
            b.Write(this.PtrTileGfx);
            b.Write(this.Tiles);
            b.Write(this.TileAnims);
            b.Write(this.PtrTileAnims);
            b.Write(this.SeaLevel);
            b.Write(this.PtrMiniMap);
            b.Write(this.Unknown1);
            b.Write(this.Pad1);
            b.Write(this.Pad2);
            b.Write(this.Pad3);
            b.Write(this.Pad4);
        }
    }
}
