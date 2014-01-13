namespace Mappy.Models
{
    using System.Collections.Generic;

    using Mappy.Data;

    public interface IFeatureDatabase
    {
        Feature this[string name] { get; }

        bool TryGetFeature(string name, out Feature feature);

        IEnumerable<Feature> EnumerateAll();
    }
}
