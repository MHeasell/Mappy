namespace Mappy.Operations.SelectionModel
{
    using System;

    using Mappy.Models;

    public class SelectFeatureOperation : IReplayableOperation
    {
        private readonly ISelectionModel model;

        private readonly Guid id;

        public SelectFeatureOperation(ISelectionModel model, Guid id)
        {
            this.model = model;
            this.id = id;
        }

        public void Execute()
        {
            this.model.SelectFeature(this.id);
        }

        public void Undo()
        {
            this.model.DeselectFeature(this.id);
        }
    }
}
