namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
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
                    var worker = (BackgroundWorker)sender;
                    var hpis = LoadingUtils.EnumerateSearchHpis().ToList();

                    var records = new List<FeatureRecord>();
                    var bitmaps = new Dictionary<string, GafFrame>();
                    var renders = new Dictionary<string, OffsetBitmap>();

                    // load records
                    int fileCount = 0;
                    foreach (string file in hpis)
                    {
                        if (worker.CancellationPending)
                        {
                            args.Cancel = true;
                            return;
                        }

                        records.AddRange(LoadFeatureTdfsFromHapi(file));
                        int progress = (++fileCount * 100) / hpis.Count;
                        worker.ReportProgress((progress * 33) / 100);
                    }

                    // load bitmaps
                    var filenameMap = records
                        .GroupBy(x => x.AnimFileName.ToLower())
                        .ToDictionary(x => x.Key, x => (IList<FeatureRecord>)x.ToList());
                    fileCount = 0;
                    foreach (string file in hpis)
                    {
                        if (worker.CancellationPending)
                        {
                            args.Cancel = true;
                            return;
                        }

                        foreach (var e in LoadFeatureBitmapsFromHapi(file, filenameMap))
                        {
                            bitmaps[e.Key] = e.Value;
                        }

                        int progress = (++fileCount * 100) / hpis.Count;
                        worker.ReportProgress(33 + ((progress * 33) / 100));
                    }

                    // load renders
                    var objectMap = records
                        .GroupBy(x => x.ObjectName.ToLower())
                        .ToDictionary(x => x.Key, x => (IList<FeatureRecord>)x.ToList());
                    fileCount = 0;
                    foreach (string file in hpis)
                    {
                        if (worker.CancellationPending)
                        {
                            args.Cancel = true;
                            return;
                        }

                        foreach (var item in LoadFeatureRendersFromHapi(file, objectMap))
                        {
                            renders[item.Key] = item.Value;
                        }

                        int progress = (++fileCount * 100) / hpis.Count;
                        worker.ReportProgress(66 + ((progress * 33) / 100));
                    }

                    // last 1%, create the feature objects
                    var features = LoadFeatureObjects(records, bitmaps, renders, p);

                    worker.ReportProgress(100);

                    args.Result = features.ToList();
                };

            bg.WorkerReportsProgress = true;
            bg.WorkerSupportsCancellation = true;

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

            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                foreach (var e in LoadFeatureBitmapsFromHapi(file, filenameMap))
                {
                    yield return e;
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, OffsetBitmap>> LoadFeatureRenders(IEnumerable<FeatureRecord> features)
        {
            var objectMap = features
                .GroupBy(x => x.ObjectName.ToLower())
                .ToDictionary(x => x.Key, x => (IList<FeatureRecord>)x.ToList());

            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                foreach (var item in LoadFeatureRendersFromHapi(file, objectMap))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, OffsetBitmap>> LoadFeatureRendersFromHapi(string file, IDictionary<string, IList<FeatureRecord>> objectMap)
        {
            using (HpiReader h = new HpiReader(file))
            {
                foreach (var item in LoadFeatureRendersFromHapi(h, objectMap))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, OffsetBitmap>> LoadFeatureRendersFromHapi(HpiReader h, IDictionary<string, IList<FeatureRecord>> objectMap)
        {
            foreach (var objPath in h.GetFilesRecursive("objects3d").Select(x => x.Name))
            {
                Debug.Assert(objPath != null, "Null path in HPI listing.");

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

        private static IEnumerable<KeyValuePair<string, GafFrame>> LoadFeatureBitmapsFromHapi(
            string hapi,
            IDictionary<string, IList<FeatureRecord>> filenameFeatureMap)
        {
            using (HpiReader h = new HpiReader(hapi))
            {
                foreach (var e in LoadFeatureBitmapsFromHapi(h, filenameFeatureMap))
                {
                    yield return e;
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, GafFrame>> LoadFeatureBitmapsFromHapi(HpiReader hapi, IDictionary<string, IList<FeatureRecord>> filenameFeatureMap)
        {
            // for each anim file in the HPI
            foreach (string anim in hapi.GetFilesRecursive("anims").Select(x => x.Name))
            {
                Debug.Assert(anim != null, "Null path in HPI listing.");

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
            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                foreach (var featureRecord in LoadFeatureTdfsFromHapi(file))
                {
                    yield return featureRecord;
                }
            }
        }

        private static IEnumerable<FeatureRecord> LoadFeatureTdfsFromHapi(string file)
        {
            using (HpiReader h = new HpiReader(file))
            {
                foreach (var e in LoadFeaturesFromHapi(h))
                {
                    yield return e;
                }
            }
        }

        private static IEnumerable<FeatureRecord> LoadFeaturesFromHapi(HpiReader hapi)
        {
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
