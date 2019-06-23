namespace Mappy.Data
{
    using System.Drawing;

    public class FeatureInfo
    {
        public string Name { get; set; }

        public Size Footprint { get; set; }

        public string World { get; set; }

        public string Category { get; set; }

        public string AnimFileName { get; set; }

        public string SequenceName { get; set; }

        public string ObjectName { get; set; }

        public Maybe<Feature.ReclaimInfoStruct> ReclaimInfo { get; set; }

        public bool Permanent { get; set; }
    }
}