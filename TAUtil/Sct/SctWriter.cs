namespace TAUtil.Sct
{
    using System.IO;

    using TAUtil.Tnt;

    public class SctWriter
    {
        private readonly BinaryWriter writer;

        public SctWriter(BinaryWriter writer)
        {
            this.writer = writer;
        }

        public void WriteSct(ISctSource adapter)
        {
            SctHeader h = new SctHeader();
            h.Version = 3;
            h.Width = (uint)adapter.DataWidth;
            h.Height = (uint)adapter.DataHeight;

            h.Tiles = (uint)adapter.TileCount;

            int ptrAccumulator = SctHeader.HeaderLength;
            
            h.PtrData = (uint)ptrAccumulator;
            ptrAccumulator += sizeof(ushort) * adapter.DataWidth * adapter.DataHeight;
            
            // height data always comes after tile data
            ptrAccumulator += TileAttr.SctVersion3AttrLength * adapter.DataWidth * 2 * adapter.DataHeight * 2;

            h.PtrTiles = (uint)ptrAccumulator;
            ptrAccumulator += MapConstants.TileDataLength * adapter.TileCount;

            h.PtrMiniMap = (uint)ptrAccumulator;

            h.Write(this.writer);

            this.WriteData(adapter);
            this.WriteHeights(adapter);
            this.WriteTiles(adapter);
            this.WriteMinimap(adapter);
        }

        private void WriteMinimap(ISctSource adapter)
        {
            this.writer.Write(adapter.GetMinimap());
        }

        private void WriteTiles(ISctSource adapter)
        {
            foreach (var tile in adapter.EnumerateTiles())
            {
                this.writer.Write(tile);
            }
        }

        private void WriteHeights(ISctSource adapter)
        {
            foreach (TileAttr t in adapter.EnumerateAttrs())
            {
                t.WriteToSct(this.writer, 3);
            }
        }

        private void WriteData(ISctSource adapter)
        {
            foreach (int i in adapter.EnumerateData())
            {
                this.writer.Write((ushort)i);
            }
        }
    }
}
