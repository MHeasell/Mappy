namespace Mappy.UI.Tags
{
    using Mappy.Models;

    public class SectionTag : IMapItemTag
    {
        public SectionTag(int index)
        {
            this.Index = index;
        }

        public int Index { get; private set; }

        public void SelectItem(IMainModel model)
        {
            model.SelectTile(this.Index);
        }
    }
}