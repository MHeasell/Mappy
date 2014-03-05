namespace Mappy.Controllers.Tags
{
    using System.Drawing;

    public class FeatureTag
    {
        public FeatureTag(Point index)
        {
            this.Index = index;
        }

        public Point Index { get; private set; }
    }
}