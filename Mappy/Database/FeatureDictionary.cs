namespace Mappy.Database
{
    using System.Collections.Generic;

    using Mappy.Data;
    using Mappy.Palette;

    public class FeatureDictionary : IFeatureDatabase
    {
        private readonly IDictionary<string, Feature> records;

        public FeatureDictionary(IPalette palette)
        {
            this.records = LoadingUtils.LoadFeatures(palette);
        }

        public Feature this[string name]
        {
            get
            {
                return this.records[name];
            }
        }

        public bool TryGetFeature(string name, out Feature feature)
        {
            return this.records.TryGetValue(name, out feature);
        }

        public IEnumerable<Feature> EnumerateAll()
        {
            return this.records.Values;
        }
    }
}
