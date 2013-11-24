namespace Mappy.Operations
{
    public interface IReplayableOperation
    {
        void Execute();

        void Undo();
    }
}
