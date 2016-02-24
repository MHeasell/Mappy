namespace Mappy.UI.Tags
{
    using Mappy.Models;

    public class StartPositionTag : IMapItemTag
    {
        public StartPositionTag(int index)
        {
            this.Index = index;
        }

        public int Index { get; }

        public void SelectItem(CoreModel model)
        {
            model.SelectStartPosition(this.Index);
        }
    }
}