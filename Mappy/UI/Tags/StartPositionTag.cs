namespace Mappy.UI.Tags
{
    using Mappy.Services;

    public class StartPositionTag : IMapItemTag
    {
        public StartPositionTag(int index)
        {
            this.Index = index;
        }

        public int Index { get; }

        public void SelectItem(Dispatcher dispatcher)
        {
            dispatcher.SelectStartPosition(this.Index);
        }
    }
}