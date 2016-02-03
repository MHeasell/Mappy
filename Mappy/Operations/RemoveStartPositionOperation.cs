namespace Mappy.Operations
{
    using System.Drawing;

    using Mappy.Models;

    public class RemoveStartPositionOperation : IReplayableOperation
    {
        private Point? oldPosition;

        public RemoveStartPositionOperation(IMapModel map, int index)
        {
            this.Map = map;
            this.Index = index;
        }

        public IMapModel Map { get; }

        public int Index { get; }

        public void Execute()
        {
            this.oldPosition = this.Map.Attributes.GetStartPosition(this.Index);
            this.Map.Attributes.SetStartPosition(this.Index, null);
        }

        public void Undo()
        {
            this.Map.Attributes.SetStartPosition(this.Index, this.oldPosition);
        }
    }
}
