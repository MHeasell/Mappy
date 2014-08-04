namespace Mappy.Operations
{
    using System.Drawing;
    using Data;

    using Mappy.Collections;
    using Mappy.Models;
    using Mappy.Operations.SelectionModel;

    public static class OperationFactory
    {
        public static IReplayableOperation CreateDeselectAndMergeOperation(ISelectionModel model)
        {
            var deselectOp = new DeselectOperation(model);
            var mergeOp = CreateFlattenOperation(model);
            return new CompositeOperation(deselectOp, mergeOp);
        }

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

        public static IReplayableOperation CreateClippedLiftAreaOperation(
            IMapModel map,
            int x,
            int y,
            int width,
            int height)
        {
            Rectangle rect = new Rectangle(x, y, width, height);
            rect.Intersect(new Rectangle(0, 0, map.Tile.TileGrid.Width, map.Tile.TileGrid.Height));

            return CreateLiftAreaOperation(map, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static IReplayableOperation CreateLiftAreaOperation(IMapModel map, int x, int y, int width, int height)
        {
            // copy the target area to a new tile
            var tile = new MapTile(width, height);
            GridMethods.Copy(map.Tile.TileGrid, tile.TileGrid, x, y, 0, 0, width, height);
            GridMethods.Copy(map.Tile.HeightGrid, tile.HeightGrid, x * 2, y * 2, 0, 0, width * 2, height * 2);

            var positionedTile = new Positioned<IMapTile>(tile, new Point(x, y));

            var addOp = new AddFloatingTileOperation(map, positionedTile);
            var clearBitmapOp = new FillAreaOperation<Bitmap>(map.Tile.TileGrid, x, y, width, height, null);
            var clearHeightOp = new FillAreaOperation<int>(map.Tile.HeightGrid, x * 2, y * 2, width * 2, height * 2, 0);

            return new CompositeOperation(addOp, clearBitmapOp, clearHeightOp);
        }
    }
}
