namespace Mappy.Operations
{
    using Mappy.Collections;

    public class TileAreaOperation<T> : IReplayableOperation
    {
        private readonly IGrid<T> source;

        private readonly IGrid<T> target;

        private readonly int x;

        private readonly int y;

        private readonly int width;

        private readonly int height;

        private IGrid<T> oldContents;

        public TileAreaOperation(IGrid<T> source, IGrid<T> target, int x, int y, int width, int height)
        {
            this.source = source;
            this.target = target;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void Execute()
        {
            this.oldContents = new Grid<T>(this.width, this.height);
            GridMethods.Copy(this.target, this.oldContents, this.x, this.y, 0, 0, this.width, this.height);

            GridMethods.FillRepeat(this.target, this.x, this.y, this.width, this.height, source);
        }

        public void Undo()
        {
            GridMethods.Copy(this.oldContents, this.target, this.x, this.y);
        }
    }
}
