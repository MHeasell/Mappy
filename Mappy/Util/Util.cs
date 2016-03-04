namespace Mappy.Util
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    using Geometry;

    using Hjg.Pngcs;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.Models;
    using Mappy.Properties;
    using Mappy.Util.ImageSampling;

    using TAUtil.Gdi.Palette;

    public static class Util
    {
        public static Bitmap ExportHeightmap(IGrid<int> heights)
        {
            var bmp = new Bitmap(heights.Width, heights.Height, PixelFormat.Format32bppArgb);
            var data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            unsafe
            {
                int* pointer = (int*)data.Scan0;
                int i = 0;
                foreach (int h in heights)
                {
                    pointer[i++] = Color.FromArgb(h, h, h).ToArgb();
                }
            }

            bmp.UnlockBits(data);

            return bmp;
        }

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

        public static int Clamp(int val, int min, int max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static float Clamp(float val, float min, float max)
        {
            return Math.Min(max, Math.Max(min, val));
        }

        public static double Clamp(double val, double min, double max)
        {
            return Math.Min(max, Math.Max(min, val));
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
            worker.DoWork += (sender, args) =>
                {
                    var w = (BackgroundWorker)sender;
                    var m = (IReadOnlyMapModel)args.Argument;

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

                        var wrapper = new NearestNeighbourPaletteWrapper(
                            new BilinearWrapper(map, width, height),
                            PaletteFactory.TAPalette);

                        Bitmap b = new Bitmap(wrapper.Width, wrapper.Height);
                        using (var g = Graphics.FromImage(b))
                        {
                            g.FillRectangle(Brushes.Black, new Rectangle(0, 0, b.Width, b.Height));
                        }

                        for (int y = 0; y < wrapper.Height; y++)
                        {
                            if (w.CancellationPending)
                            {
                                args.Cancel = true;
                                return;
                            }

                            for (int x = 0; x < wrapper.Width; x++)
                            {
                                b.SetPixel(x, y, wrapper.GetPixel(x, y));
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
                    b.SetPixel(x, y, map.GetPixel(x, y));
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

            var wrapper = new NearestNeighbourPaletteWrapper(
                new BilinearWrapper(map, width, height),
                PaletteFactory.TAPalette);
            return ToBitmap(wrapper);
        }

        public static bool WriteMapImage(Stream s, IGrid<Bitmap> map, Action<int> reportProgress, Func<bool> shouldCancel)
        {
            using (var adapter = new NonTrimmedMapPixelImageAdapter(map))
            {
                return WritePixelImageAsPng(s, adapter, reportProgress, shouldCancel);
            }
        }

        public static bool WritePixelImageAsPng(Stream s, IPixelImage img, Action<int> reportProgress, Func<bool> shouldCancel)
        {
            var imgInfo = new ImageInfo(img.Width, img.Height, 8, true);

            var writer = new PngWriter(s, imgInfo);

            for (int y = 0; y < img.Height; y++)
            {
                if (shouldCancel())
                {
                    return false;
                }

                var line = new ImageLine(imgInfo);
                for (int x = 0; x < img.Width; x++)
                {
                    var c = img.GetPixel(x, y);
                    var offset = x * 4;
                    line.Scanline[offset] = c.R;
                    line.Scanline[offset + 1] = c.G;
                    line.Scanline[offset + 2] = c.B;
                    line.Scanline[offset + 3] = c.A;
                }

                writer.WriteRow(line, y);

                reportProgress((y * 100) / img.Height);
            }

            writer.End();

            return true;
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

            int w = (int)Math.Ceiling(boundingBox.Width) + 1;
            int h = (int)Math.Ceiling(boundingBox.Height) + 1;

            Bitmap b = new Bitmap(w, h);

            var offset = boundingBox.MinXY;

            var g = Graphics.FromImage(b);

            g.TranslateTransform((float)(-offset.X), (float)(-offset.Y));

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

        public static Grid<int> ReadHeightmap(Bitmap bmp)
        {
            var data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            var grid = new Grid<int>(bmp.Width, bmp.Height);
            var len = bmp.Width * bmp.Height;

            unsafe
            {
                int* ptr = (int*)data.Scan0;
                for (int i = 0; i < len; i++)
                {
                    var c = Color.FromArgb(ptr[i]);
                    grid[i] = c.R;
                }
            }

            bmp.UnlockBits(data);

            return grid;
        }

        private static Rectangle2D ComputeBoundingBox(IEnumerable<Line2D> lines)
        {
            return ComputeBoundingBox(ToPoints(lines));
        }

        private static IEnumerable<Vector2D> ToPoints(IEnumerable<Line2D> lines)
        {
            foreach (var l in lines)
            {
                yield return l.Start;
                yield return l.End;
            }
        }

        private static Rectangle2D ComputeBoundingBox(IEnumerable<Vector2D> points)
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

            return Rectangle2D.FromMinMax(minX, minY, maxX, maxY);
        }

        private static Vector2D ProjectPoint(Vector3D point)
        {
            point /= Math.Pow(2, 16);

            point.X *= -1;

            return new Vector2D(
                point.X,
                point.Z - (point.Y / 2.0));
        }

        private static Line2D ProjectLine(Line3D line)
        {
            return new Line2D(
                ProjectPoint(line.Start),
                ProjectPoint(line.End));
        }
    }
}
