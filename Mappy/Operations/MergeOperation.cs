namespace Mappy.Operations
{
    using Mappy.Collections;

    public class MergeOperation<T> : IReplayableOperation
    {
        private readonly IGrid<T> baseGrid;
        private readonly IGrid<T> mergeGrid;

        private readonly int sourceX;
        private readonly int sourceY;

        private readonly int destX;
        private readonly int destY;

        private readonly int width;
        private readonly int height;

        private IGrid<T> oldContents;

        public MergeOperation(IGrid<T> baseGrid, IGrid<T> mergeGrid, int sourceX, int sourceY, int destX, int destY, int width, int height)
        {
            this.baseGrid = baseGrid;
            this.mergeGrid = mergeGrid;

            this.sourceX = sourceX;
            this.sourceY = sourceY;
            this.destX = destX;
            this.destY = destY;
            this.width = width;
            this.height = height;

            this.oldContents = new Grid<T>(width, height);
            for (int dy = 0; dy < height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    this.oldContents[dx, dy] = baseGrid[destX + dx, destY + dy];
                }
            }
        }

        public void Execute()
        {
            this.baseGrid.Merge(this.mergeGrid, this.sourceX, this.sourceY, this.destX, this.destY, this.width, this.height);
        }

        public void Undo()
        {
            this.baseGrid.Merge(this.oldContents, this.destX, this.destY);
        }
    }
}
