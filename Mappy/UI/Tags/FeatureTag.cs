namespace Mappy.UI.Tags
{
    using System;

    using Mappy.Services;

    public class FeatureTag : IMapItemTag
    {
        public FeatureTag(Guid featureId)
        {
            this.FeatureId = featureId;
        }

        public Guid FeatureId { get; }

        public void SelectItem(Dispatcher dispatcher)
        {
            dispatcher.SelectFeature(this.FeatureId);
        }
    }
}