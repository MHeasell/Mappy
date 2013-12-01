namespace TAUtil.Tdf
{
    using System.Collections.Generic;
    using System.IO;

    public class TdfParser
    {
        private readonly TextReader reader;

        public TdfParser(TextReader reader)
        {
            this.reader = reader;
        }

        public TdfNode Load()
        {
            TdfNode root = new TdfNode();

            string line;
            while ((line = this.ReadNextLine()) != null)
            {
                TdfNode block = new TdfNode(this.ParseBlockName(line));
                this.ReadBlockBody(block);
                root.Keys[block.Name] = block;
            }

            return root;
        }

        private string ReadNextLine()
        {
            string line = this.reader.ReadLine();

            if (line == null)
            {
                return null;
            }

            int commentIndex = line.IndexOf("//");
            if (commentIndex != -1)
            {
                line = line.Remove(commentIndex);
            }

            line = line.Trim();
            if (line.Equals(string.Empty))
            {
                return this.ReadNextLine();
            }

            return line;
        }

        private void ReadBlockBody(TdfNode node)
        {
            string openBracket = this.ReadNextLine();
            if (!openBracket.Equals("{"))
            {
                throw new IOException("failed to parse tdf");
            }

            string line;
            while (!(line = this.ReadNextLine()).Equals("}"))
            {
                if (line.StartsWith("["))
                {
                    TdfNode subBlock = new TdfNode(this.ParseBlockName(line));
                    this.ReadBlockBody(subBlock);
                    node.Keys[subBlock.Name] = subBlock;
                }
                else
                {
                    var entry = this.ParseBlockLine(line);
                    node.Entries[entry.Key] = entry.Value;
                }
            }
        }

        private string ParseBlockName(string nameLine)
        {
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
            return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
        }
    }
}
