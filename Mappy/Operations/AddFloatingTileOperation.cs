namespace Mappy.Operations
{
    using Data;
    using Mappy.Models;

    public class AddFloatingTileOperation : IReplayableOperation
    {
        private readonly IMapModel map;
        private readonly Positioned<IMapTile> tile;

        public AddFloatingTileOperation(IMapModel map, Positioned<IMapTile> tile)
        {
            this.map = map;
            this.tile = tile;
        }

        public void Execute()
        {
            this.map.FloatingTiles.Add(this.tile);
        }

        public void Undo()
        {
            this.map.FloatingTiles.Remove(this.tile);
        }
    }
}
