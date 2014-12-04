namespace TAUtil.Tdf
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class TdfRecursiveParser
    {
        private readonly TextReader reader;

        private readonly ITdfNodeAdapter adapter;

        private int currentLine = 1;

        private int currentColumn = 1;

        public TdfRecursiveParser(TextReader reader, ITdfNodeAdapter adapter)
        {
            this.reader = new TdfTextReader(reader);
            this.adapter = adapter;
        }

        public void Load()
        {
            this.OptionalWhitespace();

            while (!this.Accept(-1))
            {
                this.Block();
                this.OptionalWhitespace();
            }
        }

        private void Block()
        {
            var title = this.BlockTitle();

            this.adapter.BeginBlock(title);

            this.OptionalWhitespace();
            this.BlockBody();

            this.adapter.EndBlock();
        }

        private string BlockTitle()
        {
            this.Expect('[');
            var s = this.BlockName();
            this.Expect(']');

            return s;
        }

        private string BlockName()
        {
            StringBuilder s = new StringBuilder();

            char next = (char)this.Next();
            while (this.AcceptNot(']'))
            {
                s.Append(next);
                next = (char)this.Next();
            }

            return s.ToString();
        }

        private void BlockBody()
        {
            this.Expect('{');
            this.OptionalWhitespace();

            while (!this.Accept('}'))
            {
                this.BlockBodyEntry();
                this.OptionalWhitespace();
            }
        }

        private void BlockBodyEntry()
        {
            if (this.Next() == '[')
            {
                this.Block();
            }
            else
            {
                this.Property();
            }
        }

        private void Property()
        {
            var name = this.PropertyName();
            this.Expect('=');
            var value = this.PropertyValue();
            this.Terminator();

            this.adapter.AddProperty(name, value);
        }

        private void Terminator()
        {
            if (!this.AcceptAny(';', '\n'))
            {
                this.Error();
            }
        }

        private string PropertyName()
        {
            var sb = new StringBuilder();
            char next = (char)this.Next();

            if (!this.AcceptNotAny('=', '\n', ';'))
            {
                this.Error();
            }

            sb.Append(next);
            next = (char)this.Next();

            while (this.AcceptNotAny('=', '\n', ';'))
            {
                sb.Append(next);
                next = (char)this.Next();
            }

            return sb.ToString();
        }

        private string PropertyValue()
        {
            var sb = new StringBuilder();

            char next = (char)this.Next();

            while (this.AcceptNotAny('\n', ';'))
            {
                sb.Append(next);
                next = (char)this.Next();
            }

            return sb.ToString();
        }

        private void OptionalWhitespace()
        {
            while (this.AcceptWhitespace())
            {
                // keep accepting
            }
        }

        private bool AcceptWhitespace()
        {
            return this.AcceptAny(' ', '\n', '\t');
        }

        private int Next()
        {
            return this.reader.Peek();
        }

        private void Consume()
        {
            this.currentColumn++;

            if (this.Next() == '\n')
            {
                this.currentLine++;
                this.currentColumn = 1;
            }

            this.reader.Read();
        }

        private void Error()
        {
            char badChar = (char)this.Next();
            var msg = string.Format(
                "Unexpected character '{0}' on line {1} column {2}.",
                Regex.Escape(badChar.ToString(CultureInfo.InvariantCulture)),
                this.currentLine,
                this.currentColumn);
            throw new ParseException(msg);
        }

        private void Expect(int token)
        {
            if (!this.Accept(token))
            {
                this.Error();
            }
        }

        private bool Accept(int token)
        {
            if (this.Next() != token)
            {
                return false;
            }

            this.Consume();
            return true;
        }

        private bool AcceptNot(int token)
        {
            if (this.Next() == token)
            {
                return false;
            }

            this.Consume();
            return true;
        }

        private bool AcceptAny(params int[] tokens)
        {
            if (tokens.Any(x => x == this.Next()))
            {
                this.Consume();
                return true;
            }

            return false;
        }

        private bool AcceptNotAny(params int[] tokens)
        {
            if (tokens.Any(x => x == this.Next()))
            {
                return false;
            }

            this.Consume();
            return true;
        }
    }
}
