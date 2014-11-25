namespace Mappy.Database
{
    using System.Collections.Generic;

    using Mappy.Data;

    /// <summary>
    /// Features store backed by a dictionary
    /// </summary>
    public class FeatureDictionary : IFeatureDatabase
    {
        private readonly Dictionary<string, Feature> records;

        public FeatureDictionary()
        {
            this.records = new Dictionary<string, Feature>();
        }

        public Feature this[string name]
        {
            get
            {
                return this.records[name];
            }
        }

        public void AddFeature(Feature f)
        {
            this.records[f.Name] = f;
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
