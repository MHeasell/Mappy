namespace Mappy.Operations
{
    using System.Collections.Generic;
    using Data;

    public class RemoveTileOperation : IReplayableOperation
    {
        private IList<Positioned<IMapTile>> tiles;
        private int index;
        private Positioned<IMapTile> removedTile;

        public RemoveTileOperation(IList<Positioned<IMapTile>> tiles, int index)
        {
            this.tiles = tiles;
            this.index = index;
        }

        public void Execute()
        {
            this.removedTile = this.tiles[this.index];
            this.tiles.RemoveAt(this.index);
        }

        public void Undo()
        {
            this.tiles.Insert(this.index, this.removedTile);
        }
    }
}
