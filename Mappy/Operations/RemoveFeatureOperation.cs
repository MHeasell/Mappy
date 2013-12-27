namespace Mappy.Operations
{
    using Data;

    using Mappy.Collections;

    public class RemoveFeatureOperation : IReplayableOperation
    {
        private int x;
        private int y;
        private ISparseGrid<Feature> features;
        private Feature removedFeature;

        public RemoveFeatureOperation(ISparseGrid<Feature> features, int x, int y)
        {
            this.features = features;
            this.x = x;
            this.y = y;
        }

        public void Execute()
        {
            this.removedFeature = this.features.Get(this.x, this.y);
            this.features.Remove(this.x, this.y);
        }

        public void Undo()
        {
            this.features.Set(this.x, this.y, this.removedFeature);
            this.removedFeature = null;
        }
    }
}
