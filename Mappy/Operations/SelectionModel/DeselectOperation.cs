namespace Mappy.Operations.SelectionModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using Mappy.Collections;
    using Mappy.Models;

    public class DeselectOperation : IReplayableOperation
    {
        private readonly ISelectionModel model;

        private int? prevTile;

        private int? prevStart;

        private List<Guid> features;

        public DeselectOperation(ISelectionModel model)
        {
            this.model = model;
        }

        public void Execute()
        {
            this.prevTile = this.model.SelectedTile;
            this.features = new List<Guid>(this.model.SelectedFeatures);
            this.prevStart = this.model.SelectedStartPosition;

            this.model.DeselectAll();
        }

        public void Undo()
        {
            if (this.prevTile.HasValue)
            {
                this.model.SelectTile(this.prevTile.Value);
            }

            if (this.prevStart.HasValue)
            {
                this.model.SelectStartPosition(this.prevStart.Value);
            }

            foreach (var f in this.features)
            {
                this.model.SelectFeature(f);
            }
        }
    }
}
