namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Palette;
    using Mappy.Util;

    public class FeatureLoadingUtils
    {
        public static bool LoadFeatures(
            IPalette palette,
            Action<int> progressCallback,
            Func<bool> cancelCallback,
            out LoadResult<Feature> result)
        {
            var recordLoader = new FeatureTdfLoader();
            if (!recordLoader.LoadFiles(i => progressCallback((i * 33) / 100), cancelCallback))
            {
                result = null;
                return false;
            }

            var records = new Dictionary<string, FeatureRecord>();
            foreach (var f in recordLoader.Records)
            {
                records[f.Name] = f;
            }

            var filenameMap = records.Values
                .GroupBy(x => x.AnimFileName.ToLower())
                .ToDictionary(x => x.Key, x => (IList<FeatureRecord>)x.ToList());

            var bitmapLoader = new FeatureBitmapLoader(palette, filenameMap);
            if (!bitmapLoader.LoadFiles(i => progressCallback(33 + ((i * 33) / 100)), cancelCallback))
            {
                result = null;
                return false;
            }

            var frames = new Dictionary<string, OffsetBitmap>();
            foreach (var f in bitmapLoader.Records)
            {
                frames[f.Key] = f.Value;
            }

            var objectMap = records.Values
                .GroupBy(x => x.ObjectName.ToLower())
                .ToDictionary(x => x.Key, x => (IList<FeatureRecord>)x.ToList());

            var renderLoader = new FeatureRenderLoader(objectMap);
            if (!renderLoader.LoadFiles(i => progressCallback(66 + ((i * 33) / 100)), cancelCallback))
            {
                result = null;
                return false;
            }

            var renders = new Dictionary<string, OffsetBitmap>();
            foreach (var r in renderLoader.Records)
            {
                renders[r.Key] = r.Value;
            }

            var features = LoadFeatureObjects(records.Values, frames, renders).ToList();
            progressCallback(100);

            result = new LoadResult<Feature>
                {
                    Records = features,
                    Errors =
                        recordLoader.HpiErrors
                            .Concat(bitmapLoader.HpiErrors)
                            .Concat(renderLoader.HpiErrors)
                            .GroupBy(x => x.HpiPath)
                            .Select(x => x.First())
                            .ToList(),
                    FileErrors =
                        recordLoader.FileErrors
                            .Concat(bitmapLoader.FileErrors)
                            .Concat(renderLoader.FileErrors)
                            .ToList(),
                };
            return true;
        }

        private static IEnumerable<Feature> LoadFeatureObjects(
            IEnumerable<FeatureRecord> records,
            IDictionary<string, OffsetBitmap> frames,
            IDictionary<string, OffsetBitmap> objects)
        {
            foreach (var record in records)
            {
                Bitmap image;
                int offsetX = 0;
                int offsetY = 0;

                if (frames.ContainsKey(record.Name))
                {
                    var frame = frames[record.Name];
                    image = frame.Bitmap;
                    offsetX = -frame.OffsetX;
                    offsetY = -frame.OffsetY;
                }
                else if (objects.ContainsKey(record.Name))
                {
                    var render = objects[record.Name];
                    image = render.Bitmap;
                    offsetX = -render.OffsetX;
                    offsetY = -render.OffsetY;
                }
                else
                {
                    // no graphic for this feature... bail
                    image = Mappy.Properties.Resources.nofeature;
                }

                yield return new Feature
                    {
                        Name = record.Name,
                        Image = image,
                        Offset = new Point(offsetX, offsetY),
                        Footprint = new Size(record.FootprintX, record.FootprintY),
                        World = record.World,
                        Category = record.Category
                    };
            }
        }
    }
}
