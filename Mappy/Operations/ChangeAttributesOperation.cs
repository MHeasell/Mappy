namespace Mappy.Operations
{
    using Mappy.Models;

    public class ChangeAttributesOperation : IReplayableOperation
    {
        private MapAttributesResult oldAttrs;

        public ChangeAttributesOperation(IMapModel map, MapAttributesResult newAttributes)
        {
            this.Map = map;
            this.NewAttributes = newAttributes;
        }

        public IMapModel Map { get; private set; }

        public MapAttributesResult NewAttributes { get; private set; }

        public void Execute()
        {
            this.oldAttrs = MapAttributesResult.FromModel(this.Map);
            this.NewAttributes.MergeInto(this.Map);
        }

        public void Undo()
        {
            this.oldAttrs.MergeInto(this.Map);
        }
    }
}
