namespace Mappy.Operations
{
    using System.Collections.Generic;
    using System.Linq;

    public class CompositeOperation : IReplayableOperation
    {
        private readonly IEnumerable<IReplayableOperation> ops;

        public CompositeOperation(IEnumerable<IReplayableOperation> ops)
        {
            this.ops = ops;
        }

        public CompositeOperation(params IReplayableOperation[] ops)
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
