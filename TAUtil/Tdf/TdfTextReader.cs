namespace TAUtil.Tdf
{
    using System;
    using System.IO;

    public class TdfTextReader : TextReader
    {
        private readonly TextReader source;

        private string line;

        private int currentColumn;

        public TdfTextReader(TextReader source)
        {
            this.source = source;
            this.GetNextLine();
        }

        public override int Read()
        {
            if (this.line == null)
            {
                return -1;
            }

            if (this.currentColumn == this.line.Length)
            {
                this.GetNextLine();
                return '\n';
            }

            return this.line[this.currentColumn++];
        }

        public override int Peek()
        {
            if (this.line == null)
            {
                return -1;
            }

            if (this.currentColumn == this.line.Length)
            {
                return '\n';
            }

            return this.line[this.currentColumn];
        }

        private void GetNextLine()
        {
            this.currentColumn = 0;

            this.line = this.source.ReadLine();
            if (this.line == null)
            {
                return;
            }

            var idx = this.line.IndexOf("//", StringComparison.Ordinal);
            if (idx != -1)
            {
                this.line = this.line.Substring(0, idx);
            }
        }
    }
}
