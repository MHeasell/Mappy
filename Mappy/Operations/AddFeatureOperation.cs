namespace Mappy.Operations
{
    using Data;

    using Mappy.Collections;

    public class AddFeatureOperation : IReplayableOperation
    {
        private readonly ISparseGrid<Feature> features;
        private readonly Feature item;
        private readonly int x;
        private readonly int y;

        private Feature oldItem;
        private bool hasOldItem;

        public AddFeatureOperation(ISparseGrid<Feature> features, Feature feature, int x, int y)
        {
            this.features = features;
            this.item = feature;
            this.x = x;
            this.y = y;
        }

        public void Execute()
        {
            this.hasOldItem = this.features.TryGetValue(this.x, this.y, out this.oldItem);
            this.features.Set(this.x, this.y, this.item);
        }

        public void Undo()
        {
            if (this.hasOldItem)
            {
                this.features.Set(this.x, this.y, this.oldItem);
            }
            else
            {
                this.features.Remove(this.x, this.y);
            }
        }
    }
}
