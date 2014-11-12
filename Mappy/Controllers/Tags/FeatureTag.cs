namespace Mappy.Controllers.Tags
{
    using System;

    using Mappy.Models;

    public class FeatureTag : IMapItemTag
    {
        public FeatureTag(Guid featureId)
        {
            this.FeatureId = featureId;
        }

        public Guid FeatureId { get; private set; }

        public void SelectItem(IMainModel model)
        {
            model.SelectFeature(this.FeatureId);
        }
    }
}