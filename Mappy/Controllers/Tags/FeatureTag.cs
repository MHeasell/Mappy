namespace Mappy.Controllers.Tags
{
    public class FeatureTag
    {
        public FeatureTag(int index)
        {
            this.Index = index;
        }

        public int Index { get; private set; }
    }
}