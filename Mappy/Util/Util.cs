namespace Mappy.Util
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    using Geometry;

    using Hjg.Pngcs;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.Models;
    using Mappy.Properties;
    using Mappy.Services;
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
                var pointer = (int*)data.Scan0;
                var i = 0;
                foreach (var h in heights)
                {
                    pointer[i++] = Color.FromArgb(h, h, h).ToArgb();
                }
            }

            bmp.UnlockBits(data);

            return bmp;
        }

        public static HashSet<Bitmap> GetUsedTiles(IMapTile tile)
        {
            var set = new HashSet<Bitmap>();
            foreach (var b in tile.TileGrid)
            {
                set.Add(b);
            }

            return set;
        }

        public static Point? ScreenToHeightIndex(IGrid<int> heightmap, Point p)
        {
            var col = p.X / 16;

            if (col < 0 || col >= heightmap.Width)
            {
                return null;
            }

            var ray = new Ray3D(new Vector3D(p.X, p.Y + 128, 255.0), new Vector3D(0.0, -0.5, -1.0));

            for (var row = heightmap.Height - 1; row >= 0; row--)
            {
                var height = heightmap.Get(col, row);
                var rect = new AxisRectangle3D(
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
            var mapping = new Dictionary<T, int>();
            for (var i = 0; i < array.Length; i++)
            {
                mapping[array[i]] = i;
            }

            return mapping;
        }

        public struct RenderMinimapArgs
        {
            public IReadOnlyMapModel MapModel { get; set; }

            public FeatureService FeatureService { get; set; }
        }

        public static BackgroundWorker RenderMinimapWorker()
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += (sender, args) =>
                {
                    var w = (BackgroundWorker)sender;
                    RenderMinimapExperimental(w, args);
                    return;
                };
            return worker;
        }

        public static void RenderMinimapExperimental(BackgroundWorker worker, DoWorkEventArgs workArgs)
        {
            var args = (RenderMinimapArgs)workArgs.Argument;

            var tileGrid = args.MapModel.Tile.TileGrid;
            var mapWidth = (tileGrid.Width * 32) - 32;
            var mapHeight = (tileGrid.Height * 32) - 128;

            int width, height;
            if (mapWidth > mapHeight)
            {
                width = 252;
                height = (int)(252 * (mapHeight / (float)mapWidth));
            }
            else
            {
                height = 252;
                width = (int)(252 * (mapWidth / (float)mapHeight));
            }

            var destImage = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.ScaleTransform(
                    ((float)width) / ((float)mapWidth),
                    ((float)height) / ((float)mapHeight));

                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    for (var y = 0; y < tileGrid.Height; ++y)
                    {
                        worker.ReportProgress((y * 100) / tileGrid.Height);

                        for (var x = 0; x < tileGrid.Width; ++x)
                        {
                            var tile = tileGrid.Get(x, y);
                            var destX = x * 32;
                            var destY = y * 32;
                            graphics.DrawImage(tile, new Rectangle(destX, destY, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel, wrapMode);
                        }
                    }

                    foreach (var featureInstance in args.MapModel.EnumerateFeatureInstances())
                    {
                        var feature = args.FeatureService.TryGetFeature(featureInstance.FeatureName);
                        feature
                            .Where(f => f.Permanent)
                            .IfSome(f =>
                        {
                            var bounds = f.GetDrawBounds(args.MapModel.Tile.HeightGrid, featureInstance.X, featureInstance.Y);
                            graphics.DrawImage(f.Image, bounds, 0, 0, f.Image.Width, f.Image.Height, GraphicsUnit.Pixel, wrapMode);
                        });
                    }
                }
            }

            Quantization.ToTAPalette(destImage);

            workArgs.Result = destImage;
        }

        public static Bitmap ToBitmap(IPixelImage map)
        {
            var b = new Bitmap(map.Width, map.Height);

            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
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

            for (var y = 0; y < img.Height; y++)
            {
                if (shouldCancel())
                {
                    return false;
                }

                var line = new ImageLine(imgInfo);
                for (var x = 0; x < img.Width; x++)
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

            var w = (int)Math.Ceiling(boundingBox.Width) + 1;
            var h = (int)Math.Ceiling(boundingBox.Height) + 1;

            var b = new Bitmap(w, h);

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
                var ptr = (int*)data.Scan0;
                for (var i = 0; i < len; i++)
                {
                    var c = Color.FromArgb(ptr[i]);
                    grid[i] = c.R;
                }
            }

            bmp.UnlockBits(data);

            return grid;
        }

        public static int ComputeMidpointHeight(IGrid<int> grid, int x, int y)
        {
            var topLeft = grid.Get(x, y);
            var topRight = grid.Get(x + 1, y);
            var bottomLeft = grid.Get(x, y + 1);
            var bottomRight = grid.Get(x + 1, y + 1);

            return (topLeft + topRight + bottomLeft + bottomRight) / 4;
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
            var minX = double.PositiveInfinity;
            var maxX = double.NegativeInfinity;
            var minY = double.PositiveInfinity;
            var maxY = double.NegativeInfinity;

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
