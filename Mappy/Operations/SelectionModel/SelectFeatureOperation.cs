namespace Mappy.Operations.SelectionModel
{
    using Mappy.Collections;
    using Mappy.Models;

    public class SelectFeatureOperation : IReplayableOperation
    {
        private readonly ISelectionModel model;

        private readonly GridCoordinates coord;

        public SelectFeatureOperation(ISelectionModel model, GridCoordinates coord)
        {
            this.model = model;
            this.coord = coord;
        }

        public void Execute()
        {
            this.model.SelectFeature(this.coord);
        }

        public void Undo()
        {
            this.model.DeselectFeature(this.coord);
        }
    }
}
