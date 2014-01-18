namespace Mappy.Operations
{
    using Mappy.Collections;

    public class CopyAreaOperation<T> : IReplayableOperation
    {
        private readonly IGrid<T> source;
        private readonly IGrid<T> destination;

        private readonly int sourceX;
        private readonly int sourceY;

        private readonly int destX;
        private readonly int destY;

        private readonly int width;
        private readonly int height;

        private readonly IGrid<T> oldContents;

        public CopyAreaOperation(IGrid<T> source, IGrid<T> destination, int sourceX, int sourceY, int destX, int destY, int width, int height)
        {
            this.source = source;
            this.destination = destination;

            this.sourceX = sourceX;
            this.sourceY = sourceY;
            this.destX = destX;
            this.destY = destY;
            this.width = width;
            this.height = height;

            this.oldContents = new Grid<T>(width, height);

            GridMethods.Copy(destination, this.oldContents, destX, destY, 0, 0, width, height);
        }

        public void Execute()
        {
            GridMethods.Copy(this.source, this.destination, this.sourceX, this.sourceY, this.destX, this.destY, this.width, this.height);
        }

        public void Undo()
        {
            GridMethods.Copy(this.oldContents, this.destination, this.destX, this.destY);
        }
    }
}
