namespace TAUtil.Gdi.Palette
{
    using System;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// Wrapper for reading TA palette files.
    /// The files themselves are just 256 colors
    /// packed in binary, 4 bytes for each color: r, g, b, a
    /// </summary>
    public class BinaryPaletteReader : IDisposable
    {
        private readonly BinaryReader reader;

        public BinaryPaletteReader(BinaryReader b)
        {
            this.reader = b;
        }

        public Color ReadColor()
        {
            int r = this.reader.ReadByte();
            int g = this.reader.ReadByte();
            int b = this.reader.ReadByte();

            // alpha, ignored
            this.reader.ReadByte();

            return Color.FromArgb(r, g, b);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.reader.Dispose();
            }
        }
    }
}
