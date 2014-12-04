namespace Mappy.Util
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;

    using Geometry;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.Models;
    using Mappy.Properties;
    using Mappy.Util.ImageSampling;

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

            if (col < 0 || col >= heightmap.Width)
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

        public static BackgroundWorker RenderMinimapWorker()
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += delegate(object sender, DoWorkEventArgs args)
                {
                    var w = (BackgroundWorker)sender;
                    var m = (IMapModel)args.Argument;

                    using (var map = new MapPixelImageAdapter(m.Tile.TileGrid))
                    {
                        int width, height;

                        if (map.Width > map.Height)
                        {
                            width = 252;
                            height = (int)(252 * (map.Height / (float)map.Width));
                        }
                        else
                        {
                            height = 252;
                            width = (int)(252 * (map.Width / (float)map.Height));
                        }

                        var wrapper = new BilinearWrapper(map, width, height);

                        Bitmap b = new Bitmap(wrapper.Width, wrapper.Height);

                        for (int y = 0; y < wrapper.Height; y++)
                        {
                            if (w.CancellationPending)
                            {
                                args.Cancel = true;
                                return;
                            }

                            for (int x = 0; x < wrapper.Width; x++)
                            {
                                b.SetPixel(x, y, wrapper[x, y]);
                            }

                            int percentComplete = ((y + 1) * 100) / wrapper.Height;

                            w.ReportProgress(percentComplete);
                        }

                        args.Result = b;
                    }
                };
            return worker;
        }

        public static Bitmap ToBitmap(IPixelImage map)
        {
            Bitmap b = new Bitmap(map.Width, map.Height);

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    b.SetPixel(x, y, map[x, y]);
                }
            }

            return b;
        }

        public static Bitmap GenerateMinimap(IPixelImage map)
        {
            int width, height;

            if (map.Width > map.Height)
            {
                width = 252;
                height = (int)(252 * (map.Height / (float)map.Width));
            }
            else
            {
                height = 252;
                width = (int)(252 * (map.Width / (float)map.Height));
            }

            var wrapper = new NearestNeighbourWrapper(map, width, height);
            return ToBitmap(wrapper);
        }

        public static Bitmap GenerateMinimapLinear(IPixelImage map)
        {
            int width, height;

            if (map.Width > map.Height)
            {
                width = 252;
                height = (int)(252 * (map.Height / (float)map.Width));
            }
            else
            {
                height = 252;
                width = (int)(252 * (map.Width / (float)map.Height));
            }

            var wrapper = new BilinearWrapper(map, width, height);
            return ToBitmap(wrapper);
        }

        public static Point ToPoint(GridCoordinates g)
        {
            return new Point(g.X, g.Y);
        }

        public static GridCoordinates ToGridCoordinates(Point p)
        {
            return new GridCoordinates(p.X, p.Y);
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            TValue result;
            if (!dict.TryGetValue(key, out result))
            {
                return defaultValue;
            }

            return result;
        }

        public static OffsetBitmap RenderWireframe(IEnumerable<Line3D> edges)
        {
            var projectedLines = edges.Select(ProjectLine).ToList();
            var boundingBox = ComputeBoundingBox(projectedLines);

            int w = (int)Math.Ceiling(boundingBox.Extents.X * 2) + 1;
            int h = (int)Math.Ceiling(boundingBox.Extents.Y * 2) + 1;

            Bitmap b = new Bitmap(w, h);

            var offset = boundingBox.TopLeft;

            var g = Graphics.FromImage(b);

            g.TranslateTransform((float)(-offset.X), (float)-offset.Y);

            foreach (var l in projectedLines)
            {
                g.DrawLine(
                    Pens.Magenta,
                    (float)l.Start.X,
                    (float)l.Start.Y,
                    (float)l.End.X,
                    (float)l.End.Y);
            }

            return new OffsetBitmap(
                (int)Math.Round(offset.X),
                (int)Math.Round(offset.Y),
                b);
        }

        private static AxisRectangle3D ComputeBoundingBox(IEnumerable<Line3D> lines)
        {
            return ComputeBoundingBox(ToPoints(lines));
        }

        private static IEnumerable<Vector3D> ToPoints(IEnumerable<Line3D> lines)
        {
            foreach (var l in lines)
            {
                yield return l.Start;
                yield return l.End;
            }
        }

        private static AxisRectangle3D ComputeBoundingBox(IEnumerable<Vector3D> points)
        {
            double minX = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double minY = double.PositiveInfinity;
            double maxY = double.NegativeInfinity;

            foreach (var p in points)
            {
                if (p.X < minX)
                {
                    minX = p.X;
                }

                if (p.X > maxX)
                {
                    maxX = p.X;
                }

                if (p.Y < minY)
                {
                    minY = p.Y;
                }

                if (p.Y > maxY)
                {
                    maxY = p.Y;
                }
            }

            return AxisRectangle3D.FromTLBR(maxY, minX, minY, maxX);
        }

        private static Vector3D ProjectPoint(Vector3D point)
        {
            point /= Math.Pow(2, 16);

            point.X *= -1;

            point = new Vector3D(
                point.X,
                point.Z - (point.Y / 2.0),
                0.0);

            return point;
        }

        private static Line3D ProjectLine(Line3D line)
        {
            return new Line3D(
                ProjectPoint(line.Start),
                ProjectPoint(line.End));
        }
    }
}
