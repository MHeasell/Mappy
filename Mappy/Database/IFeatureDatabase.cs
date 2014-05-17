namespace Mappy.Database
{
    using System.Collections.Generic;

    using Mappy.Data;

    /// <summary>
    /// Defines a store for feature data.
    /// </summary>
    public interface IFeatureDatabase
    {
        Feature this[string name] { get; }

        bool TryGetFeature(string name, out Feature feature);

        IEnumerable<Feature> EnumerateAll();
    }
}
