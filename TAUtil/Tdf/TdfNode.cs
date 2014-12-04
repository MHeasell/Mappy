namespace TAUtil.Tdf
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class TdfNode
    {
        public const int IndentationLevel = 4;

        public TdfNode()
            : this(null)
        {
        }

        public TdfNode(string name)
        {
            this.Name = name;
            this.Keys = new Dictionary<string, TdfNode>(StringComparer.OrdinalIgnoreCase);
            this.Entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; set; }

        public Dictionary<string, TdfNode> Keys { get; private set; }

        public Dictionary<string, string> Entries { get; private set; }

        public static TdfNode LoadTdf(TextReader reader)
        {
            var adapter = new TdfNodeAdapter();
            var parser = new TdfParser(reader, adapter);
            parser.Load();
            return adapter.RootNode;
        }

        public void WriteTdf(Stream s)
        {
            StreamWriter wr = new StreamWriter(s);
            this.WriteTdf(wr, 0);
            wr.Flush();
        }

        protected void WriteTdf(StreamWriter writer, int depth)
        {
            string indent = new string(' ', depth * TdfNode.IndentationLevel);
            string indent2 = new string(' ', (depth + 1) * TdfNode.IndentationLevel);

            // write out the header
            writer.Write(indent);
            writer.WriteLine("[{0}]", this.Name);

            // open the body
            writer.Write(indent2);
            writer.WriteLine("{");

            // write the body
            // first, variables and their values
            foreach (var e in this.Entries)
            {
                writer.Write(indent2);
                writer.WriteLine("{0}={1};", e.Key, e.Value);
            }

            // then subkeys
            foreach (var e in this.Keys)
            {
                e.Value.WriteTdf(writer, depth + 1);
            }

            // close the body
            writer.Write(indent2);
            writer.WriteLine("}");
        }
    }
}
