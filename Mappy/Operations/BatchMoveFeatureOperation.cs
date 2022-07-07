namespace Mappy.Operations
{
    using System;
    using System.Collections.Generic;

    using Mappy.Models;

    public class BatchMoveFeatureOperation : IReplayableOperation
    {
        private readonly IMapModel map;

        private readonly ISet<Guid> ids;

        private readonly int x;

        private readonly int y;

        public BatchMoveFeatureOperation(
            IMapModel map,
            IEnumerable<Guid> ids,
            int x,
            int y)
        {
            this.map = map;
            this.ids = new HashSet<Guid>(ids);
            this.x = x;
            this.y = y;
        }

        public void Execute()
        {
            foreach (var id in this.ids)
            {
                var inst = this.map.GetFeatureInstance(id);
                this.map.UpdateFeatureInstanceInBatch(inst.Translate(this.x, this.y), this.ids);
            }
        }

        public void Undo()
        {
            foreach (var id in this.ids)
            {
                var inst = this.map.GetFeatureInstance(id);
                this.map.UpdateFeatureInstance(inst.Translate(-this.x, -this.y));
            }
        }

        public bool CanCombine(BatchMoveFeatureOperation other)
        {
            return this.ids.SetEquals(other.ids);
        }

        public BatchMoveFeatureOperation Combine(BatchMoveFeatureOperation other)
        {
            return new BatchMoveFeatureOperation(
                this.map,
                this.ids,
                this.x + other.x,
                this.y + other.y);
        }
    }
}
