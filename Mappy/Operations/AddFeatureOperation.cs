namespace Mappy.Operations
{
    using Mappy.Models;

    public class AddFeatureOperation : IReplayableOperation
    {
        private readonly IMapModel map;
        private readonly FeatureInstance item;

        public AddFeatureOperation(IMapModel map, FeatureInstance feature)
        {
            this.map = map;
            this.item = feature;
        }

        public void Execute()
        {
            this.map.AddFeatureInstance(this.item);
        }

        public void Undo()
        {
            this.map.RemoveFeatureInstance(this.item.Id);
        }
    }
}
