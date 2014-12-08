namespace TAUtil.Tdf
{
    using System.IO;

    public class TdfLexingReader : TextReader
    {
        private readonly TextReader reader;

        private int nextChar;

        private int nextNextChar;

        private int outputChar;

        private int internalCurrentLine;

        private int internalCurrentColumn;

        public TdfLexingReader(TextReader reader)
        {
            this.reader = reader;
            this.nextChar = this.reader.Read();
            this.nextNextChar = this.reader.Read();

            this.internalCurrentLine = 1;
            this.internalCurrentColumn = 1;

            this.GetNextOutputChar();
        }

        public int CurrentLine { get; set; }

        public int CurrentColumn { get; set; }

        public override int Peek()
        {
            return this.outputChar;
        }

        public override int Read()
        {
            var ch = this.outputChar;
            this.GetNextOutputChar();
            return ch;
        }

        private void GetNextOutputChar()
        {
            while (this.AcceptComment())
            {
                // keep consuming comments
            }

            // take a snapshot of our internal position here
            this.CurrentColumn = this.internalCurrentColumn;
            this.CurrentLine = this.internalCurrentLine;

            // Try to accept newline and output a normalized version.
            if (this.AcceptNewline())
            {
                this.outputChar = '\n';
                return;
            }

            this.outputChar = this.nextChar;
            this.Consume();
        }

        private bool AcceptNewline()
        {
            if (this.Accept('\n'))
            {
                this.internalCurrentColumn = 1;
                this.internalCurrentLine++;
                return true;
            }

            if (this.Accept('\r'))
            {
                this.Accept('\n');
                this.internalCurrentColumn = 1;
                this.internalCurrentLine++;
                return true;
            }

            return false;
        }

        private bool Accept(int chr)
        {
            if (this.nextChar == chr)
            {
                this.Consume();
                return true;
            }

            return false;
        }

        private bool AcceptComment()
        {
            if (this.nextChar == '/')
            {
                if (this.nextNextChar == '/')
                {
                    this.ConsumeUntilNewline();
                    return true;
                }

                if (this.nextNextChar == '*')
                {
                    this.ConsumeUntilAfterEndBlockComment();
                }
            }

            return false;
        }

        private void ConsumeUntilAfterEndBlockComment()
        {
            while (this.nextChar != -1)
            {
                // explicitly accept newlines for line counting
                if (this.AcceptNewline())
                {
                    continue;
                }

                if (this.nextChar == '*' && this.nextNextChar == '/')
                {
                    this.Consume();
                    this.Consume();
                    break;
                }

                this.Consume();
            }
        }

        private void ConsumeUntilNewline()
        {
            while (this.nextChar != '\r' && this.nextChar != '\n' && this.nextChar != -1)
            {
                this.Consume();
            }
        }

        private void Consume()
        {
            this.internalCurrentColumn++;
            this.nextChar = this.nextNextChar;
            this.nextNextChar = this.reader.Read();
        }
    }
}
