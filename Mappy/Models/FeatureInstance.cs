namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.Services;

    public sealed class FeatureInstance
    {
        private static readonly Feature DefaultFeatureRecord = new Feature
        {
            Name = "default",
            Offset = new Point(0, 0),
            Footprint = new Size(1, 1),
            Image = Mappy.Properties.Resources.nofeature
        };

        public FeatureInstance(Guid id, string featureName, int x, int y)
            : this(id, featureName, new GridCoordinates(x, y))
        {
        }

        public FeatureInstance(Guid id, string featureName, GridCoordinates location)
        {
            this.Id = id;
            this.FeatureName = featureName;
            this.Location = location;
            this.BaseFeature = FeatureService.TryGetFeature(featureName).Or(DefaultFeatureRecord);
        }

        public static FeatureService FeatureService { get; set; }

        public Guid Id { get; }

        public Feature BaseFeature { get; }

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
