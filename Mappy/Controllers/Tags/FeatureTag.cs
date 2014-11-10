namespace Mappy.Controllers.Tags
{
    using Mappy.Collections;
    using Mappy.Models;

    public class FeatureTag : IMapItemTag
    {
        public FeatureTag(GridCoordinates index)
        {
            this.Index = index;
        }

        public GridCoordinates Index { get; private set; }

        public void SelectItem(IMainModel model)
        {
            model.SelectFeature(this.Index);
        }
    }
}