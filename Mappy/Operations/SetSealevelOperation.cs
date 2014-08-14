namespace Mappy.Operations
{
    using Mappy.Models;

    public class SetSealevelOperation : IReplayableOperation
    {
        private readonly IMapModel map;

        private readonly int value;

        private int previousValue;

        public SetSealevelOperation(IMapModel map, int value)
        {
            this.map = map;
            this.value = value;
            this.previousValue = this.map.SeaLevel;
        }

        public void Execute()
        {
            this.map.SeaLevel = this.value;
        }

        public void Undo()
        {
            this.map.SeaLevel = this.previousValue;
        }

        public SetSealevelOperation Combine(SetSealevelOperation other)
        {
            var newOp = new SetSealevelOperation(this.map, other.value);
            newOp.previousValue = this.previousValue;
            return newOp;
        }
    }
}