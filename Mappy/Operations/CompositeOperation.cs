namespace Mappy.Operations
{
    using System.Collections.Generic;
    using System.Linq;

    public class CompositeOperation : IReplayableOperation
    {
        private IEnumerable<IReplayableOperation> ops;

        public CompositeOperation(IReplayableOperation op1, IReplayableOperation op2)
            : this(new IReplayableOperation[] { op1, op2 })
        {
        }

        public CompositeOperation(IEnumerable<IReplayableOperation> ops)
        {
            this.ops = ops;
        }

        public void Execute()
        {
            foreach (IReplayableOperation op in this.ops)
            {
                op.Execute();
            }
        }

        public void Undo()
        {
            foreach (IReplayableOperation op in this.ops.Reverse())
            {
                op.Undo();
            }
        }
    }
}
