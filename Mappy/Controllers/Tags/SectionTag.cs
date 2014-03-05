namespace Mappy.Controllers.Tags
{
    public class SectionTag
    {
        public SectionTag(int index)
        {
            this.Index = index;
        }

        public int Index { get; private set; }
    }
}