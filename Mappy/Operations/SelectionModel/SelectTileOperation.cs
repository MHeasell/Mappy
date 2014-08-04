namespace Mappy.Operations.SelectionModel
{
    using Mappy.Models;

    public class SelectTileOperation : IReplayableOperation
    {
        private readonly ISelectionModel model;

        private readonly int tileId;

        private int? previousSelection;

        public SelectTileOperation(ISelectionModel model, int tileId)
        {
            this.model = model;
            this.tileId = tileId;
        }

        public void Execute()
        {
            this.previousSelection = this.model.SelectedTile;
            this.model.SelectTile(this.tileId);
        }

        public void Undo()
        {
            if (this.previousSelection.HasValue)
            {
                this.model.SelectTile(this.previousSelection.Value);
            }
            else
            {
                this.model.DeselectTile();
            }
        }
    }
}
