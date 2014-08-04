namespace Mappy.Operations
{
    using System.Collections.Generic;
    using System.Linq;

    using Mappy.Collections;
    using Mappy.Data;

    public class BatchMoveFeatureOperation : IReplayableOperation
    {
        private readonly BindingSparseGrid<Feature> grid;

        private readonly ISet<GridCoordinates> coords;

        private readonly int x;

        private readonly int y;

        public BatchMoveFeatureOperation(
            BindingSparseGrid<Feature> grid,
            ISet<GridCoordinates> coords,
            int x,
            int y)
        {
            this.grid = grid;
            this.coords = coords;
            this.x = x;
            this.y = y;
        }

        public void Execute()
        {
            // BUG: this assumes that the destination of any feature
            //      does not contain another feature
            //      that is also about to be moved.
            foreach (var i in this.coords)
            {
                this.grid.Move(i.X, i.Y, i.X + this.x, i.Y + this.y);
            }
        }

        public void Undo()
        {
            foreach (var i in this.coords)
            {
                this.grid.Move(i.X + this.x, i.Y + this.y, i.X, i.Y);
            }
        }

        public ISet<GridCoordinates> GetTranslatedCoords()
        {
            return new HashSet<GridCoordinates>(
                this.coords.Select(i => new GridCoordinates(i.X + this.x, i.Y + this.y)));
        }

        public BatchMoveFeatureOperation Combine(BatchMoveFeatureOperation other)
        {
            return new BatchMoveFeatureOperation(
                this.grid,
                this.coords,
                this.x + other.x,
                this.y + other.y);
        }
    }
}
