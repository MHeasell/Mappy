namespace Mappy.IO.Gaf
{
    using TAUtil.Gaf;

    public class GafEntryArrayAdapter : IGafReaderAdapter
    {
        private int currentEntryIndex;

        private GafEntry currentEntry;

        private bool frameRead;

        private GafFrame currentFrame;

        private int frameDepth;

        public GafEntry[] Entries { get; private set; }

        public void BeginRead(long entryCount)
        {
            this.Entries = new GafEntry[entryCount];
        }

        public void BeginEntry(string name, int frameCount)
        {
            var entry = new GafEntry();
            entry.Name = name;
            entry.Frames = new GafFrame[frameCount];
            this.currentEntry = entry;

            this.frameRead = false;
        }

        public void BeginFrame(int x, int y, int width, int height, int transparencyIndex, int subframeCount)
        {
            if (this.frameRead)
            {
                // Don't read any frame after the first.
                return;
            }

            this.frameDepth++;

            if (this.frameDepth > 1)
            {
                // Ignore subframes.
                return;
            }

            var f = new GafFrame();
            f.OffsetX = x;
            f.OffsetY = y;
            f.Width = width;
            f.Height = height;
            f.TransparencyIndex = (byte)transparencyIndex;

            this.currentFrame = f;
        }

        public void SetFrameData(byte[] data)
        {
            if (this.frameRead)
            {
                // Ignore any frame after the first.
                return;
            }

            if (this.frameDepth > 1)
            {
                // Ignore subframes.
                return;
            }

            this.currentFrame.Data = data;
        }

        public void EndFrame()
        {
            if (this.frameRead)
            {
                // Ignore any frame after the first.
                return;
            }

            this.frameDepth--;

            if (this.frameDepth == 0)
            {
                this.currentEntry.Frames = new[] { this.currentFrame };
                this.frameRead = true;
            }
        }

        public void EndEntry()
        {
            this.Entries[this.currentEntryIndex++] = this.currentEntry;
        }

        public void EndRead()
        {
            // Do nothing.
        }
    }
}
