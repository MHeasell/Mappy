namespace Mappy
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public static class GridUtil
    {
        public static Rectangle GetCoveringRect(Rectangle rect, Size tileSize, Size gridSize)
        {
            int startTileX = Math.Max(0, rect.Left / tileSize.Width);
            int startTileY = Math.Max(0, rect.Top / tileSize.Height);
            int endTileX = Math.Min(
                gridSize.Width - 1,
                (rect.Right / tileSize.Width) + 1);
            int endTileY = Math.Min(
                gridSize.Height - 1,
                (rect.Bottom / tileSize.Height) + 1);

            return Rectangle.FromLTRB(startTileX, startTileY, endTileX, endTileY);
        }

        public static IEnumerable<Point> EnumerateCoveringIndices(Rectangle rect, Size tileSize, Size gridSize)
        {
            Rectangle coveringRect = GridUtil.GetCoveringRect(rect, tileSize, gridSize);

            for (int y = coveringRect.Top; y <= coveringRect.Bottom; y++)
            {
                for (int x = coveringRect.Left; x <= coveringRect.Right; x++)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }
}
