namespace Mappy
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public static class GridUtil
    {
        public static Rectangle GetCoveringRect(Rectangle rect, Size tileSize, Size gridSize)
        {
            var startTileX = Math.Max(0, rect.Left / tileSize.Width);
            var startTileY = Math.Max(0, rect.Top / tileSize.Height);
            var endTileX = Math.Min(
                gridSize.Width - 1,
                (rect.Right / tileSize.Width) + 1);
            var endTileY = Math.Min(
                gridSize.Height - 1,
                (rect.Bottom / tileSize.Height) + 1);

            return Rectangle.FromLTRB(startTileX, startTileY, endTileX, endTileY);
        }

        public static IEnumerable<Point> EnumerateCoveringIndices(Rectangle rect, Size tileSize, Size gridSize)
        {
            var coveringRect = GetCoveringRect(rect, tileSize, gridSize);

            for (var y = coveringRect.Top; y <= coveringRect.Bottom; y++)
            {
                for (var x = coveringRect.Left; x <= coveringRect.Right; x++)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }
}
