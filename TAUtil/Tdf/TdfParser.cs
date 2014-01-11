namespace TAUtil.Tdf
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class TdfParser
    {
        private readonly TextReader reader;
        private readonly ITdfNodeAdapter adapter;

        private int lineCount;

        public TdfParser(TextReader reader, ITdfNodeAdapter adapter)
        {
            this.reader = reader;
            this.adapter = adapter;
        }

        public TdfNode Load()
        {
            TdfNode root = new TdfNode();

            string line;
            while ((line = this.ReadNextLine()) != null)
            {
                this.adapter.BeginBlock(this.ParseBlockName(line));
                this.ReadBlockBody();
                this.adapter.EndBlock();
            }

            return root;
        }

        private string ReadNextLine()
        {
            string line;
            do
            {
                line = this.reader.ReadLine();
                this.lineCount++;

                if (line == null)
                {
                    return null;
                }

                int commentIndex = line.IndexOf("//", StringComparison.Ordinal);
                if (commentIndex != -1)
                {
                    line = line.Remove(commentIndex);
                }

                line = line.Trim();
            }
            while (string.Equals(line, string.Empty, StringComparison.Ordinal));

            return line;
        }

        private void ReadBlockBody()
        {
            string openBracket = this.ReadNextLine();
            if (!string.Equals(openBracket, "{", StringComparison.Ordinal))
            {
                this.RaiseError("{", openBracket);
            }

            string line;
            while (!string.Equals(line = this.ReadNextLine(), "}", StringComparison.Ordinal))
            {
                if (line.StartsWith("[", StringComparison.Ordinal))
                {
                    this.adapter.BeginBlock(this.ParseBlockName(line));
                    this.ReadBlockBody();
                    this.adapter.EndBlock();
                }
                else
                {
                    var entry = this.ParseBlockLine(line);
                    this.adapter.AddProperty(entry.Key, entry.Value);
                }
            }
        }

        private string ParseBlockName(string nameLine)
        {
            if (!nameLine.StartsWith("[", StringComparison.Ordinal)
                || !nameLine.EndsWith("]", StringComparison.Ordinal))
            {
                this.RaiseError("[<name>]", nameLine);
            }

            return nameLine.Substring(1, nameLine.Length - 2);
        }

        private KeyValuePair<string, string> ParseBlockLine(string line)
        {
            // Chomp ending semicolon.
            // Some files are missing semicolons at the end of a statement,
            // so we assume that statements are terminated by newlines.
            int i = line.IndexOf(';');
            if (i != -1)
            {
                line = line.Remove(i);
            }

            string[] parts = line.Split(new[] { '=' }, 2);

            if (parts.Length < 2)
            {
                this.RaiseError("<key>=<value>", line);
            }

            return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
        }

        private void RaiseError(string message)
        {
            throw new ParseException(string.Format("line {0}: {1}", this.lineCount, message));
        }

        private void RaiseError(string expected, string actual)
        {
            this.RaiseError(string.Format("Expected {0}, got {1}", expected, actual));
        }
    }
}
