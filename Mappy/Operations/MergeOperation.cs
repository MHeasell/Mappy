namespace Mappy.Operations
{
    using Mappy.Grids;

    public class MergeOperation<T> : IReplayableOperation
    {
        private IGrid<T> baseGrid;
        private IGrid<T> mergeGrid;

        private IGrid<T> oldContents;

        private int sourceX;
        private int sourceY;

        private int destX;
        private int destY;

        private int width;
        private int height;

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
                    this.oldContents.Set(dx, dy, baseGrid.Get(destX + dx, destY + dy));
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
