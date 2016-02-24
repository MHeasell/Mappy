namespace Mappy.UI.Tags
{
    using Mappy.Models;

    public class SectionTag : IMapItemTag
    {
        public SectionTag(int index)
        {
            this.Index = index;
        }

        public int Index { get; }

        public void SelectItem(CoreModel model)
        {
            model.SelectTile(this.Index);
        }
    }
}