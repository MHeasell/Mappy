namespace Mappy.Operations
{
    using System.Collections.Generic;
    using System.Linq;

    public class CompositeOperation : IReplayableOperation
    {
        private readonly IList<IReplayableOperation> ops;

        public CompositeOperation(IList<IReplayableOperation> ops)
        {
            this.ops = ops;
        }

        public CompositeOperation(params IReplayableOperation[] ops)
        {
            this.ops = ops;
        }

        public void Execute()
        {
            foreach (var op in this.ops)
            {
                op.Execute();
            }
        }

        public void Undo()
        {
            foreach (var op in this.ops.Reverse())
            {
                op.Undo();
            }
        }
    }
}
