namespace Mappy.Controllers.Tags
{
    using Mappy.Models;

    public class StartPositionTag : IMapItemTag
    {
        public StartPositionTag(int index)
        {
            this.Index = index;
        }

        public int Index { get; private set; }

        public void SelectItem(IMainModel model)
        {
            model.SelectStartPosition(this.Index);
        }
    }
}