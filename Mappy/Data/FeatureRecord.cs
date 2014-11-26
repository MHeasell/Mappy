namespace Mappy.Data
{
    using Mappy.Util;

    using TAUtil.Tdf;

    public class FeatureRecord
    {
        public string Name { get; set; }

        public string World { get; set; }

        public string Category { get; set; }

        public int FootprintX { get; set; }

        public int FootprintY { get; set; }

        public string AnimFileName { get; set; }

        public string SequenceName { get; set; }

        public string ObjectName { get; set; }

        public static FeatureRecord FromTdfNode(TdfNode n)
        {
            return new FeatureRecord
                {
                    Name = n.Name,
                    World = n.Entries.GetOrDefault("world", string.Empty),
                    Category = n.Entries.GetOrDefault("category", string.Empty),
                    FootprintX = TdfConvert.ToInt32(n.Entries.GetOrDefault("footprintx", "0")),
                    FootprintY = TdfConvert.ToInt32(n.Entries.GetOrDefault("footprintx", "0")),
                    AnimFileName = n.Entries.GetOrDefault("filename", string.Empty),
                    SequenceName = n.Entries.GetOrDefault("seqname", string.Empty),
                    ObjectName = n.Entries.GetOrDefault("object", string.Empty)
                };
        }
    }
}
