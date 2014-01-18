namespace Mappy.Operations
{
    using System;
    using Data;

    using Mappy.Collections;

    public class MoveFeatureOperation : IReplayableOperation
    {
        public MoveFeatureOperation(ISparseGrid<Feature> grid, int startX, int startY, int destX, int destY)
        {
            this.Grid = grid;
            this.StartX = startX;
            this.DestX = destX;
            this.DestY = destY;
            this.StartY = startY;
        }

        public int StartX { get; private set; }

        public int StartY { get; private set; }

        public int DestX { get; private set; }

        public int DestY { get; private set; }

        public ISparseGrid<Feature> Grid { get; private set; }

        public void Execute()
        {
            Feature f = this.Grid[this.StartX, this.StartY];
            this.Grid.Remove(this.StartX, this.StartY);
            this.Grid[this.DestX, this.DestY] = f;
        }

        public void Undo()
        {
            Feature f = this.Grid[this.DestX, this.DestY];
            this.Grid.Remove(this.DestX, this.DestY);
            this.Grid[this.StartX, this.StartY] = f;
        }

        public MoveFeatureOperation Combine(MoveFeatureOperation other)
        {
            if (other.Grid != this.Grid)
            {
                throw new ArgumentException("Operation must be on same grid");
            }

            if (other.StartX != this.DestX || other.StartY != this.DestY)
            {
                throw new ArgumentException("Input operation must start where this one finished");
            }

            return new MoveFeatureOperation(
                    this.Grid,
                    this.StartX,
                    this.StartY,
                    other.DestX,
                    other.DestY);
        }
    }
}