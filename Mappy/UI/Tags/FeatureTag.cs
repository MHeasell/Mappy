namespace Mappy.UI.Tags
{
    using System;

    using Mappy.Models;

    public class FeatureTag : IMapItemTag
    {
        public FeatureTag(Guid featureId)
        {
            this.FeatureId = featureId;
        }

        public Guid FeatureId { get; }

        public void SelectItem(CoreModel model)
        {
            model.SelectFeature(this.FeatureId);
        }
    }
}