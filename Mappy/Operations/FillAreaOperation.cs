namespace Mappy.Operations
{
    using Mappy.Collections;

    public class FillAreaOperation<T> : IReplayableOperation
    {
        private readonly IGrid<T> target;

        private readonly int x;

        private readonly int y;

        private readonly int width;

        private readonly int height;

        private readonly T fillValue;

        private IGrid<T> oldContents; 

        public FillAreaOperation(IGrid<T> target, int x, int y, int width, int height, T fillValue)
        {
            this.target = target;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.fillValue = fillValue;
        }

        public void Execute()
        {
            this.oldContents = new Grid<T>(this.width, this.height);
            GridMethods.Copy(this.target, this.oldContents, this.x, this.y, 0, 0, this.width, this.height);
            GridMethods.Fill(this.target, this.x, this.y, this.width, this.height, this.fillValue);
        }

        public void Undo()
        {
            GridMethods.Copy(this.oldContents, this.target, this.x, this.y);
        }
    }
}
