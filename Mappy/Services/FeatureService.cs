namespace Mappy.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Maybe;

    public class FeatureService
    {
        private readonly Dictionary<string, Feature> records;

        public FeatureService()
        {
            this.records = new Dictionary<string, Feature>();
        }

        public event EventHandler FeaturesChanged;

        public Feature Get(string name) => this.records[name];

        public void AddFeature(Feature f)
        {
            this.records[f.Name] = f;
        }

        public void NotifyChanges()
        {
            this.FeaturesChanged?.Invoke(this, EventArgs.Empty);
        }

        public Maybe<Feature> TryGetFeature(string name)
        {
            Feature f;
            return this.records.TryGetValue(name, out f)
                ? Maybe.Some(f)
                : Maybe.None<Feature>();
        }

        public IEnumerable<Feature> EnumerateAll() => this.records.Select(x => x.Value);

        public IEnumerable<string> EnumerateWorlds() =>
            this.records
                .Select(x => x.Value.World)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<string> EnumerateCategories(string world) =>
            this.records
                .Select(x => x.Value)
                .Where(x => x.World == world)
                .Select(x => x.Category)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<Feature> EnumerateFeatures(string world, string category) =>
            this.records
                .Select(x => x.Value)
                .Where(x => string.Equals(x.World, world, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(x.Category, category, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
    }
}
