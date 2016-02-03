namespace Mappy.Models
{
    using System;

    using Mappy.Collections;

    public sealed class FeatureInstance
    {
        public FeatureInstance(Guid id, string featureName, int x, int y)
            : this(id, featureName, new GridCoordinates(x, y))
        {
        }

        public FeatureInstance(Guid id, string featureName, GridCoordinates location)
        {
            this.Id = id;
            this.FeatureName = featureName;
            this.Location = location;
        }

        public Guid Id { get; }

        public string FeatureName { get; }

        public GridCoordinates Location { get; }

        public int X => this.Location.X;

        public int Y => this.Location.Y;

        public FeatureInstance Translate(int x, int y)
        {
            return new FeatureInstance(this.Id, this.FeatureName, this.X + x, this.Y + y);
        }
    }
}
