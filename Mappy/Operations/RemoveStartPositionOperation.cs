namespace Mappy.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;

    using Mappy.Models;

    public class RemoveStartPositionOperation : IReplayableOperation
    {
        private Point? oldPosition;

        public RemoveStartPositionOperation(IMapModel map, int index)
        {
            this.Map = map;
            this.Index = index;
        }

        public IMapModel Map { get; private set; }

        public int Index { get; private set; }

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
