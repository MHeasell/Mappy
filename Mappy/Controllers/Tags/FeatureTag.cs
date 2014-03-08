namespace Mappy.Controllers.Tags
{
    using Mappy.Collections;

    public class FeatureTag
    {
        public FeatureTag(GridCoordinates index)
        {
            this.Index = index;
        }

        public GridCoordinates Index { get; private set; }
    }
}