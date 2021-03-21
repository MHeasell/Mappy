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
    using ImageMagick;
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
                    RenderHighQualityMinimap(w, args);
                };
            return worker;
        }

        public struct FeatureInfo
        {
            public Bitmap Image { get; set; }
            public Point Location { get; set; }
        }

        public static IEnumerable<U> Choose<T, U>(this IEnumerable<T> coll, Func<T, Maybe<U>> f)
        {
            return coll.SelectMany(item => f(item).Match(x => new[] { x }, () => new U[] { }));
        }

        public static void RenderHighQualityMinimap(BackgroundWorker w, DoWorkEventArgs workArgs)
        {
            var args = (RenderMinimapArgs)workArgs.Argument;

            var featuresList = args.MapModel.EnumerateFeatureInstances()
                .Choose(f =>
                    args.FeatureService.TryGetFeature(f.FeatureName)
                        .Where(rec => rec.Permanent)
                        .Select(rec => new FeatureInfo
                            {
                                Image = rec.Image,
                                Location = rec.GetDrawBounds(args.MapModel.Tile.HeightGrid, f.X, f.Y).Location
                            }))
                .ToList();
            featuresList.Sort((a, b) =>
            {
                if (a.Location.Y - b.Location.Y != 0)
                {
                    return a.Location.Y - b.Location.Y;
                }

                return a.Location.X - b.Location.X;
            });

            var tileGrid = args.MapModel.Tile.TileGrid;
            var mapWidth = (tileGrid.Width * 32) - 32;
            var mapHeight = (tileGrid.Height * 32) - 128;

            var tileCache = new Dictionary<Bitmap, BitmapData>();

            var tempPath = Path.GetTempFileName();
            var resizedPath = Path.GetTempFileName();
            var imgInfo = new ImageInfo(mapWidth, mapHeight, 8, true);
            try
            {
                using (var tempFileStream = File.OpenWrite(tempPath))
                {
                    var writer = new PngWriter(tempFileStream, imgInfo);

                    var inProgressFeatures = new List<FeatureInfo>();
                    var currentFeatureIndex = 0;

                    for (var sourcePixelY = 0; sourcePixelY < mapHeight; ++sourcePixelY)
                    {
                        w.ReportProgress(sourcePixelY * 100 / mapHeight);
                        if (w.CancellationPending)
                        {
                            workArgs.Cancel = true;
                            return;
                        }

                        var tileY = sourcePixelY / 32;
                        var line = new ImageLine(imgInfo);
                        for (var tileX = 0; tileX < tileGrid.Width - 1; ++tileX)
                        {
                            var tile = tileGrid.Get(tileX, tileY);
                            if (!tileCache.TryGetValue(tile, out var tileData))
                            {
                                tileData = tile.LockBits(new Rectangle(0, 0, tile.Width, tile.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                                tileCache[tile] = tileData;
                            }

                            var inTileY = sourcePixelY % 32;

                            for (var inTileX = 0; inTileX < 32; ++inTileX)
                            {
                                var sourcePixelX = (tileX * 32) + inTileX;
                                var pngOffset = sourcePixelX * 4;
                                unsafe
                                {
                                    var ptr = (int*)tileData.Scan0;
                                    var color = Color.FromArgb(ptr[(inTileY * 32) + inTileX]);
                                    line.Scanline[pngOffset] = color.R;
                                    line.Scanline[pngOffset + 1] = color.G;
                                    line.Scanline[pngOffset + 2] = color.B;
                                    line.Scanline[pngOffset + 3] = color.A;
                                }
                            }
                        }

                        var nextInProgressFeaturesList = new List<FeatureInfo>();

                        // Add feature bitmap data to the row
                        var inProgressFeatureIndex = 0;
                        while (true)
                        {
                            // Find the next feature
                            FeatureInfo nextFeature;
                            if (currentFeatureIndex < featuresList.Count && inProgressFeatureIndex < inProgressFeatures.Count && featuresList[currentFeatureIndex].Location.Y <= sourcePixelY)
                            {
                                nextFeature = featuresList[currentFeatureIndex].Location.X < inProgressFeatures[inProgressFeatureIndex].Location.X
                                    ? featuresList[currentFeatureIndex++]
                                    : inProgressFeatures[inProgressFeatureIndex++];
                            }
                            else if (currentFeatureIndex < featuresList.Count && featuresList[currentFeatureIndex].Location.Y <= sourcePixelY)
                            {
                                nextFeature = featuresList[currentFeatureIndex++];
                            }
                            else if (inProgressFeatureIndex < inProgressFeatures.Count)
                            {
                                nextFeature = inProgressFeatures[inProgressFeatureIndex++];
                            }
                            else
                            {
                                break;
                            }

                            if (!tileCache.TryGetValue(nextFeature.Image, out var featureImageData))
                            {
                                featureImageData = nextFeature.Image.LockBits(new Rectangle(0, 0, nextFeature.Image.Width, nextFeature.Image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                                tileCache[nextFeature.Image] = featureImageData;
                            }

                            var imageRect = new Rectangle(nextFeature.Location, nextFeature.Image.Size);

                            var coveringRect = new Rectangle(0, 0, mapWidth, mapHeight);
                            coveringRect.Intersect(imageRect);

                            var featureBitmapY = sourcePixelY - imageRect.Y;
                            for (var dx = 0; dx < coveringRect.Width; ++dx)
                            {
                                var featureBitmapX = (coveringRect.X - imageRect.X) + dx;
                                var sourcePixelX = coveringRect.X + dx;
                                var pngOffset = sourcePixelX * 4;
                                unsafe
                                {
                                    var ptr = (int*)featureImageData.Scan0;
                                    var color = Color.FromArgb(ptr[(featureBitmapY * imageRect.Width) + featureBitmapX]);
                                    if (color.A > 0)
                                    {
                                        line.Scanline[pngOffset] = color.R;
                                        line.Scanline[pngOffset + 1] = color.G;
                                        line.Scanline[pngOffset + 2] = color.B;
                                        line.Scanline[pngOffset + 3] = color.A;
                                    }
                                }
                            }

                            // read it again next row
                            if (sourcePixelY + 1 < imageRect.Y + imageRect.Height)
                            {
                                nextInProgressFeaturesList.Add(nextFeature);
                            }
                        }

                        inProgressFeatures = nextInProgressFeaturesList;

                        writer.WriteRow(line, sourcePixelY);
                    }

                    writer.End();
                }

                using (var image = new MagickImage(tempPath))
                {
                    image.GammaCorrect(1.0 / 2.2);
                    image.Resize(252, 252);
                    image.GammaCorrect(2.2);
                    image.Map(PaletteFactory.TAPalette.Select(c => new MagickColor(c.R, c.G, c.B, c.A)), new QuantizeSettings { DitherMethod = DitherMethod.No });
                    image.Write(resizedPath);
                }

                using (var outputTemp = new Bitmap(resizedPath))
                {
                    workArgs.Result = new Bitmap(outputTemp);
                }
            }
            finally
            {
                foreach (var entry in tileCache)
                {
                    entry.Key.UnlockBits(entry.Value);
                }

                File.Delete(tempPath);
                File.Delete(resizedPath);
            }
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
