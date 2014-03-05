namespace Mappy.Controllers.Tags
{
    public class StartPositionTag
    {
        public StartPositionTag(int index)
        {
            this.Index = index;
        }

        public int Index { get; private set; }
    }
}