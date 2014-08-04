namespace Mappy.Operations.SelectionModel
{
    using Mappy.Models;

    public class SelectStartPositionOperation : IReplayableOperation
    {
        private readonly ISelectionModel model;

        private readonly int index;

        private int? previousSelection;

        public SelectStartPositionOperation(ISelectionModel model, int index)
        {
            this.model = model;
            this.index = index;
        }

        public void Execute()
        {
            this.previousSelection = this.model.SelectedStartPosition;
            this.model.SelectStartPosition(this.index);
        }

        public void Undo()
        {
            if (this.previousSelection.HasValue)
            {
                this.model.SelectStartPosition(this.previousSelection.Value);
            }
            else
            {
                this.model.DeselectStartPosition();
            }
        }
    }
}
