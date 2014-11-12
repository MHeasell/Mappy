namespace Mappy.Operations
{
    using System;

    using Mappy.Collections;
    using Mappy.Models;

    public class RemoveFeatureOperation : IReplayableOperation
    {
        private readonly IMapModel map;

        private readonly Guid id;

        private FeatureInstance removedFeature;

        public RemoveFeatureOperation(IMapModel map, Guid id)
        {
            this.map = map;
            this.id = id;
        }

        public void Execute()
        {
            this.removedFeature = this.map.GetFeatureInstance(this.id);
            this.map.RemoveFeatureInstance(this.id);
        }

        public void Undo()
        {
            this.map.AddFeatureInstance(this.removedFeature);
        }
    }
}
