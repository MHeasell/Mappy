namespace TAUtil
{
    using System;
    using System.Drawing;
    using System.IO;

    public class PaletteReader : IDisposable
    {
        private readonly TextReader reader;

        public PaletteReader(TextReader reader)
        {
            this.reader = reader;

            string magic1 = reader.ReadLine();
            string magic2 = reader.ReadLine();

            if (magic1 != "JASC-PAL" || magic2 != "0100")
            {
                throw new ParseException("Unrecognised palette format");
            }

            string entries = reader.ReadLine();

            if (entries == null)
            {
                throw new ParseException("File ended prematurely");
            }

            this.ColorsCount = int.Parse(entries);
        }

        ~PaletteReader()
        {
            this.Dispose(false);
        }

        public int ColorsCount { get; private set; }

        public Color ReadColor()
        {
            string line = this.reader.ReadLine();
            if (line == null)
            {
                throw new EndOfStreamException();
            }

            string[] fields = line.Split(' ');
            return Color.FromArgb(
                int.Parse(fields[0]),
                int.Parse(fields[1]),
                int.Parse(fields[2]));
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.reader.Dispose();
            }
        }
    }
}
