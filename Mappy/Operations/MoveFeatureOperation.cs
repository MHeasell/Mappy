namespace Mappy.Operations
{
    using System;
    using Data;

    using Mappy.Collections;

    public class MoveFeatureOperation : IReplayableOperation
    {
        public MoveFeatureOperation(IBindingSparseGrid<Feature> grid, int startX, int startY, int destX, int destY)
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

        public IBindingSparseGrid<Feature> Grid { get; private set; }

        public void Execute()
        {
            this.Grid.Move(this.StartX, this.StartY, this.DestX, this.DestY);
        }

        public void Undo()
        {
            this.Grid.Move(this.DestX, this.DestY, this.StartX, this.StartY);
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