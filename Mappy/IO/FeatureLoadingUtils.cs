namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Palette;
    using Mappy.Util;

    using TAUtil.Gaf;
    using TAUtil.Hpi;
    using TAUtil.Tdf;

    public class FeatureLoadingUtils
    {
        public static BackgroundWorker LoadFeaturesBackgroundWorker()
        {
            var bg = new BackgroundWorker();
            bg.DoWork += delegate(object sender, DoWorkEventArgs args)
                {
                    var p = (IPalette)args.Argument;
                    args.Result = LoadFeatures(p).ToList();
                };
            return bg;
        }

        /// <summary>
        /// Loads the database of features from disk
        /// </summary>
        /// <param name="palette"></param>
        /// <returns></returns>
        public static IEnumerable<Feature> LoadFeatures(IPalette palette)
        {
            var records = new Dictionary<string, FeatureRecord>();
            foreach (var r in LoadFeatureTdfs())
            {
                records[r.Name] = r;
            }

            var frames = new Dictionary<string, GafFrame>();
            foreach (var f in LoadFeatureBitmaps(records.Values))
            {
                frames[f.Key] = f.Value;
            }

            var renders = new Dictionary<string, OffsetBitmap>();
            foreach (var r in LoadFeatureRenders(records.Values))
            {
                renders[r.Key] = r.Value;
            }

            return LoadFeatureObjects(records.Values, frames, renders, palette);
        }

        private static IEnumerable<Feature> LoadFeatureObjects(
            IEnumerable<FeatureRecord> records,
            IDictionary<string, GafFrame> frames,
            IDictionary<string, OffsetBitmap> objects,
            IPalette palette)
        {
            var maskedPalette = new TransparencyMaskedPalette(palette);
            var deserializer = new BitmapDeserializer(maskedPalette);

            foreach (var record in records)
            {
                Bitmap image = null;
                int offsetX = 0;
                int offsetY = 0;

                if (frames.ContainsKey(record.Name))
                {
                    var frame = frames[record.Name];
                    if (frame.Width == 0 || frame.Height == 0)
                    {
                        image = new Bitmap(50, 50);
                    }
                    else
                    {
                        maskedPalette.TransparencyIndex = frame.TransparencyIndex;
                        image = deserializer.Deserialize(frame.Data, frame.Width, frame.Height);
                    }

                    offsetX = frame.OffsetX;
                    offsetY = frame.OffsetY;
                }
                else if (objects.ContainsKey(record.Name))
                {
                    var render = objects[record.Name];
                    image = render.Bitmap;
                    offsetX = -render.OffsetX;
                    offsetY = -render.OffsetY;
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

        /// <summary>
        /// Loads the corresponding image for each feature in the given database
        /// </summary>
        /// <param name="features"></param>
        /// <returns>A mapping of feature name to image</returns>
        private static IEnumerable<KeyValuePair<string, GafFrame>> LoadFeatureBitmaps(IEnumerable<FeatureRecord> features)
        {
            var filenameMap = features
                .GroupBy(x => x.AnimFileName.ToLower())
                .ToDictionary(x => x.Key, x => (IList<FeatureRecord>)x.ToList());

            if (MappySettings.Settings.SearchPaths == null)
            {
                yield break;
            }

            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                using (HpiReader h = new HpiReader(file))
                {
                    foreach (var e in LoadFeatureBitmapsFromHapi(h, filenameMap))
                    {
                        yield return e;
                    }
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, OffsetBitmap>> LoadFeatureRenders(IEnumerable<FeatureRecord> features)
        {
            var objectMap = features
                .GroupBy(x => x.ObjectName.ToLower())
                .ToDictionary(x => x.Key, x => (IList<FeatureRecord>)x.ToList());

            if (MappySettings.Settings.SearchPaths == null)
            {
                yield break;
            }

            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                using (HpiReader h = new HpiReader(file))
                {
                    foreach (var objPath in h.GetFilesRecursive("objects3d").Select(x => x.Name))
                    {
                        var objName = Path.GetFileNameWithoutExtension(objPath).ToLowerInvariant();

                        IList<FeatureRecord> records;
                        if (!objectMap.TryGetValue(objName, out records))
                        {
                            continue;
                        }

                        using (var b = h.ReadFile(objPath))
                        {
                            var r = new ModelEdgeReader();
                            r.Read(b);
                            var wire = Util.RenderWireframe(r.Edges);
                            foreach (var record in records)
                            {
                                yield return new KeyValuePair<string, OffsetBitmap>(record.Name, wire);
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, GafFrame>> LoadFeatureBitmapsFromHapi(HpiReader hapi, IDictionary<string, IList<FeatureRecord>> filenameFeatureMap)
        {
            Dictionary<string, GafFrame> bitmaps = new Dictionary<string, GafFrame>();

            // for each anim file in the HPI
            foreach (string anim in hapi.GetFilesRecursive("anims").Select(x => x.Name))
            {
                IList<FeatureRecord> records;
                if (!filenameFeatureMap.TryGetValue(Path.GetFileNameWithoutExtension(anim).ToLower(), out records))
                {
                    // Skip if no anims needed from this file.
                    continue;
                }

                // extract and read the file
                GafEntry[] gaf;
                using (var b = new GafReader(hapi.ReadFile(anim)))
                {
                    gaf = b.Read();
                }

                // retrieve the anim for each record
                foreach (var record in records)
                {
                    var sequenceName = record.SequenceName;
                    if (string.IsNullOrEmpty(sequenceName))
                    {
                        // Skip if this record has no sequence name.
                        continue;
                    }

                    var entry = gaf.FirstOrDefault(
                        x => string.Equals(x.Name, sequenceName, StringComparison.OrdinalIgnoreCase));
                    if (entry != null)
                    {
                        yield return new KeyValuePair<string, GafFrame>(record.Name, entry.Frames[0]);
                    }
                }
            }
        }

        private static IEnumerable<FeatureRecord> LoadFeatureTdfs()
        {
            Dictionary<string, TdfNode> features = new Dictionary<string, TdfNode>();

            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                using (HpiReader h = new HpiReader(file))
                {
                    foreach (var e in LoadFeaturesFromHapi(h))
                    {
                        yield return e;
                    }
                }
            }
        }

        private static IEnumerable<FeatureRecord> LoadFeaturesFromHapi(HpiReader hapi)
        {
            Dictionary<string, TdfNode> features = new Dictionary<string, TdfNode>();
            foreach (string feature in hapi.GetFilesRecursive("features").Select(x => x.Name))
            {
                if (feature.EndsWith(".tdf", StringComparison.OrdinalIgnoreCase))
                {
                    TdfNode n;
                    using (StreamReader sr = new StreamReader(hapi.ReadTextFile(feature)))
                    {
                        n = TdfNode.LoadTdf(sr);
                    }

                    foreach (var e in n.Keys.Values)
                    {
                        yield return FeatureRecord.FromTdfNode(e);
                    }
                }
            }
        }
    }
}
