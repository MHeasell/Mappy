namespace Mappy.Operations
{
    using System.Drawing;
    using Data;
    using Mappy.Models;

    public static class OperationFactory
    {
        public static IReplayableOperation CreateClipMergeTileOperation(IMapTile src, IMapTile dst, int x, int y)
        {
            // construct the destination target
            Rectangle rect = new Rectangle(x, y, src.TileGrid.Width, src.TileGrid.Height);

            // clip to boundaries
            rect.Intersect(new Rectangle(0, 0, dst.TileGrid.Width, dst.TileGrid.Height));

            int srcX = rect.X - x;
            int srcY = rect.Y - y;

            return new CompositeOperation(
                new CopyAreaOperation<Bitmap>(src.TileGrid, dst.TileGrid, srcX, srcY, rect.X, rect.Y, rect.Width, rect.Height),
                new CopyAreaOperation<int>(src.HeightGrid, dst.HeightGrid, srcX * 2, srcY * 2, rect.X * 2, rect.Y * 2, rect.Width * 2, rect.Height * 2));
        }

        public static IReplayableOperation CreateFlattenOperation(IMapModel model)
        {
            IReplayableOperation[] arr = new IReplayableOperation[model.FloatingTiles.Count + 1];

            int i = 0;
            foreach (Positioned<IMapTile> t in model.FloatingTiles)
            {
                arr[i++] = OperationFactory.CreateClipMergeTileOperation(t.Item, model.Tile, t.Location.X, t.Location.Y);
            }

            arr[i] = new ClearTilesOperation(model);

            return new CompositeOperation(arr);
        }

        public static IReplayableOperation CreateMergeSectionOperation(IMapModel map, int index)
        {
            var tile = map.FloatingTiles[index];

            var mergeOp = OperationFactory.CreateClipMergeTileOperation(tile.Item, map.Tile, tile.Location.X, tile.Location.Y);

            var removeOp = new RemoveTileOperation(map.FloatingTiles, index);

            return new CompositeOperation(mergeOp, removeOp);
        }
    }
}
