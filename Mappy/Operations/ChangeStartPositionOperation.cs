namespace Mappy.Operations
{
    using System;
    using System.Drawing;

    using Mappy.Models;

    public class ChangeStartPositionOperation : IReplayableOperation
    {
        private Point? oldPosition;

        public ChangeStartPositionOperation(IMapModel map, int index, Point newPosition)
        {
            this.Map = map;
            this.Index = index;
            this.NewPosition = newPosition;
        }

        public IMapModel Map { get; }

        public int Index { get; }

        public Point NewPosition { get; }

        public void Execute()
        {
            this.oldPosition = this.Map.Attributes.GetStartPosition(this.Index);
            this.Map.Attributes.SetStartPosition(this.Index, this.NewPosition);
        }

        public void Undo()
        {
            this.Map.Attributes.SetStartPosition(this.Index, this.oldPosition);
        }

        public ChangeStartPositionOperation Combine(ChangeStartPositionOperation newOp)
        {
            if (newOp.Index != this.Index)
            {
                throw new ArgumentException("other op must be for same start position");
            }

            var n = new ChangeStartPositionOperation(this.Map, this.Index, newOp.NewPosition);
            n.oldPosition = this.oldPosition;
            return n;
        }
    }
}
