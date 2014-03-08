namespace Mappy.Operations
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Mappy.Collections;
    using Mappy.Data;

    public class BatchMoveFeatureOperation : IReplayableOperation
    {
        private readonly ISparseGrid<Feature> grid;

        private readonly ISet<GridCoordinates> coords;

        private readonly int x;

        private readonly int y;

        public BatchMoveFeatureOperation(
            ISparseGrid<Feature> grid,
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
            var mapping = new Dictionary<GridCoordinates, Feature>();
            foreach (var i in this.coords)
            {
                mapping[i] = this.grid[i.X, i.Y];
                this.grid.Remove(i.X, i.Y);
            }

            foreach (var i in this.coords)
            {
                this.grid[i.X + this.x, i.Y + this.y] = mapping[i];
            }
        }

        public void Undo()
        {
            var mapping = new Dictionary<GridCoordinates, Feature>();
            foreach (var i in this.coords)
            {
                mapping[i] = this.grid[i.X + this.x, i.Y + this.y];
                this.grid.Remove(i.X + this.x, i.Y + this.y);
            }

            foreach (var i in this.coords)
            {
                this.grid[i.X, i.Y] = mapping[i];
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
