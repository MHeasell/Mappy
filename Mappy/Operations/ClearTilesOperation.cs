namespace Mappy.Operations
{
    using System.Collections.Generic;

    using Mappy.Data;
    using Mappy.Models;

    public class ClearTilesOperation : IReplayableOperation
    {
        private readonly IMapModel model;

        private IList<Positioned<IMapTile>> oldTiles;

        public ClearTilesOperation(IMapModel model)
        {
            this.model = model;
            this.oldTiles = new List<Positioned<IMapTile>>(this.model.FloatingTiles);
        }

        public void Execute()
        {
            this.model.FloatingTiles.Clear();
        }

        public void Undo()
        {
            foreach (var e in this.oldTiles)
            {
                this.model.FloatingTiles.Add(e);
            }
        }
    }
}
