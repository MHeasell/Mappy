namespace Mappy.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Data;
    using Geometry;

    using Mappy.Collections;
    using Mappy.Properties;

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
                int height = heightmap.Get(col, row);
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
    }
}
