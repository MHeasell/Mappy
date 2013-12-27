namespace Mappy.Operations
{
    using Data;

    using Mappy.Collections;

    public class AddFeatureOperation : IReplayableOperation
    {
        private ISparseGrid<Feature> features;
        private Feature item;
        private Feature oldItem;
        private bool hasOldItem;

        private int x;
        private int y;

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
