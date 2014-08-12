namespace Mappy.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Data;
    using Geometry;

    using Mappy.Collections;
    using Mappy.Models;
    using Mappy.Properties;

    using Color = System.Drawing.Color;

    public static class Util
    {
        public static HashSet<Bitmap> GetUsedTiles(IMapTile tile)
        {
            HashSet<Bitmap> set = new HashSet<Bitmap>();
            foreach (Bitmap b in tile.TileGrid)
            {
                set.Add(b);
            }

            return set;
        }

        public static Point? ScreenToHeightIndex(IGrid<int> heightmap, Point p)
        {
            int col = p.X / 16;

            if (col < 0 || col >= heightmap.Height)
            {
                return null;
            }

            Ray3D ray = new Ray3D(new Vector3D(p.X, p.Y + 128, 255.0), new Vector3D(0.0, -0.5, -1.0));

            for (int row = heightmap.Height - 1; row >= 0; row--)
            {
                int height = heightmap[col, row];
                AxisRectangle3D rect = new AxisRectangle3D(
                    new Vector3D((col * 16) + 0.5, (row * 16) + 0.5, height),
                    16.0,
                    16.0);
                double distance;
                if (rect.Intersect(ray, out distance))
                {
                    return new Point(col, row);
                }
            }

            return null;
        }

        public static double Mod(double a, double p)
        {
            return ((a % p) + p) % p;
        }

        public static int Mod(int a, int p)
        {
            return ((a % p) + p) % p;
        }

        public static Bitmap GetStartImage(int index)
        {
            switch (index)
            {
                case 1:
                    return Resources.number_1;
                case 2:
                    return Resources.number_2;
                case 3:
                    return Resources.number_3;
                case 4:
                    return Resources.number_4;
                case 5:
                    return Resources.number_5;
                case 6:
                    return Resources.number_6;
                case 7:
                    return Resources.number_7;
                case 8:
                    return Resources.number_8;
                case 9:
                    return Resources.number_9;
                case 10:
                    return Resources.number_10;
                default:
                    throw new ArgumentException("invalid index: " + index);
            }
        }

        public static IDictionary<T, int> ReverseMapping<T>(T[] array)
        {
            Dictionary<T, int> mapping = new Dictionary<T, int>();
            for (int i = 0; i < array.Length; i++)
            {
                mapping[array[i]] = i;
            }

            return mapping;
        }

        public static Bitmap GenerateMinimap(IMapModel mapModel)
        {
            int mapWidth = (mapModel.Tile.TileGrid.Width * 32) - 32;
            int mapHeight = (mapModel.Tile.TileGrid.Height * 32) - 128;

            int width, height;

            if (mapModel.Tile.TileGrid.Width > mapModel.Tile.TileGrid.Height)
            {
                width = 252;
                height = (int)(252 * (mapHeight / (float)mapWidth));
            }
            else
            {
                height = 252;
                width = (int)(252 * (mapWidth / (float)mapHeight));
            }

            Bitmap b = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int imageX = (int)((x / (float)width) * mapWidth);
                    int imageY = (int)((y / (float)height) * mapHeight);
                    b.SetPixel(x, y, GetPixel(mapModel, imageX, imageY));
                }
            }

            return b;
        }

        public static Bitmap GenerateMinimapLinear(IMapModel mapModel)
        {
            int mapWidth = mapModel.Tile.TileGrid.Width * 32;
            int mapHeight = mapModel.Tile.TileGrid.Height * 32;

            int width, height;

            if (mapModel.Tile.TileGrid.Width > mapModel.Tile.TileGrid.Height)
            {
                width = 252;
                height = (int)(252 * (mapHeight / (float)mapWidth));
            }
            else
            {
                height = 252;
                width = (int)(252 * (mapWidth / (float)mapHeight));
            }

            Bitmap b = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var sampledColor = SampleArea(x, y, width, height, mapModel);
                    var nearestNeighbour = NearestNeighbour(sampledColor, Globals.Palette);
                    b.SetPixel(x, y, nearestNeighbour);
                }
            }

            return b;
        }

        public static Point ToPoint(GridCoordinates g)
        {
            return new Point(g.X, g.Y);
        }

        public static GridCoordinates ToGridCoordinates(Point p)
        {
            return new GridCoordinates(p.X, p.Y);
        }

        private static Color NearestNeighbour(Color color, IEnumerable<Color> choices)
        {
            Color winner = new Color();
            double winningValue = double.PositiveInfinity;

            foreach (var candidate in choices)
            {
                double dist = DistanceSquared(color, candidate);
                if (dist < winningValue)
                {
                    winner = candidate;
                    winningValue = dist;
                }
            }

            return winner;
        }

        private static double DistanceSquared(Color a, Color b)
        {
            int dR = b.R - a.R;
            int dG = b.G - a.G;
            int dB = b.B - a.B;

            return (dR * dR) + (dG * dG) + (dB * dB);

        }

        private static Color SampleArea(int x, int y, int outputWidth, int outputHeight, IMapModel model)
        {
            int mapWidth = model.Tile.TileGrid.Width * 32;
            int mapHeight = model.Tile.TileGrid.Height * 32;

            int cellWidth = mapWidth / outputWidth;
            int cellHeight = mapHeight / outputHeight;

            int startX = x * cellWidth;
            int startY = y * cellHeight;

            int r = 0;
            int g = 0;
            int b = 0;

            for (int dy = 0; dy < cellHeight; dy++)
            {
                for (int dx = 0; dx < cellWidth; dx++)
                {
                    Color c = GetPixel(model, startX + dx, startY + dy);
                    r += c.R;
                    g += c.G;
                    b += c.B;
                }
            }

            int factor = cellWidth * cellHeight;

            r /= factor;
            g /= factor;
            b /= factor;

            return Color.FromArgb(r, g, b);
        }

        private static Color GetPixel(IMapModel mapModel, int x, int y)
        {
            int tileX = x / 32;
            int tileY = y / 32;

            int tilePixelX = x % 32;
            int tilePixelY = y % 32;

            foreach (Positioned<IMapTile> t in mapModel.FloatingTiles.Reverse())
            {
                Rectangle r = new Rectangle(t.Location, new Size(t.Item.TileGrid.Width, t.Item.TileGrid.Height));
                if (r.Contains(tileX, tileY))
                {
                    return t.Item.TileGrid[tileX - t.Location.X, tileY - t.Location.Y].GetPixel(tilePixelX, tilePixelY);
                }
            }

            Bitmap bitmap = mapModel.Tile.TileGrid[tileX, tileY];
            if (bitmap == null)
            {
                return Color.Black;
            }

            return bitmap.GetPixel(tilePixelX, tilePixelY);
        }
    }
}
