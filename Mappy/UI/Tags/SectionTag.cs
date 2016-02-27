namespace Mappy.UI.Tags
{
    using Mappy.Services;

    public class SectionTag : IMapItemTag
    {
        public SectionTag(int index)
        {
            this.Index = index;
        }

        public int Index { get; }

        public void SelectItem(Dispatcher dispatcher)
        {
            dispatcher.SelectTile(this.Index);
        }
    }
}