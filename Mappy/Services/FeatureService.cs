namespace Mappy.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Mappy.Data;
    using Mappy.IO;
    using Mappy.IO.Gaf;
    using Mappy.Util;

    using TAUtil._3do;
    using TAUtil.Gaf;
    using TAUtil.Gdi.Bitmap;
    using TAUtil.Hpi;

    public class FeatureService
    {
        private readonly Dictionary<string, FeatureInfo> records;

        private readonly Dictionary<string, Feature> featureCache;

        public FeatureService()
        {
            this.records = new Dictionary<string, FeatureInfo>();
            this.featureCache = new Dictionary<string, Feature>();
        }

        public event EventHandler FeaturesChanged;

        public void AddFeatures(IEnumerable<FeatureInfo> features)
        {
            foreach (var f in features)
            {
                this.AddFeature(f);
            }

            this.FeaturesChanged?.Invoke(this, EventArgs.Empty);
        }

        public Maybe<Feature> TryGetFeature(string name)
        {
            if (!this.records.TryGetValue(name, out var info))
            {
                return Maybe.None<Feature>();
            }

            return Maybe.From(
                this.EnumerateFeaturesFromInternal(new[] { new KeyValuePair<string, FeatureInfo>(name, info) })
                    .FirstOrDefault());
        }

        public IEnumerable<string> EnumerateWorlds() =>
            this.records
                .Select(x => x.Value.World)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<string> EnumerateCategories(string world) =>
            this.records
                .Select(x => x.Value)
                .Where(x => string.Equals(x.World, world, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Category)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<Feature> EnumerateFeatures(string world, string category)
        {
            return this.EnumerateFeaturesInternal(world, category).OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
        }

        private static OffsetBitmap LoadRenderFile(HpiArchive archive, HpiArchive.FileInfo file)
        {
            var fileBuffer = new byte[file.Size];
            archive.Extract(file, fileBuffer);

            using (var b = new MemoryStream(fileBuffer))
            {
                var adapter = new ModelEdgeReaderAdapter();
                var reader = new ModelReader(b, adapter);
                reader.Read();
                var wire = Util.RenderWireframe(adapter.Edges);
                return wire;
            }
        }

        private static OffsetBitmap LoadBitmap(GafEntry[] gaf, string sequenceName)
        {
                var entry = gaf.FirstOrDefault(
                    x => string.Equals(x.Name, sequenceName, StringComparison.OrdinalIgnoreCase));
                if (entry == null)
                {
                    // skip if the sequence is not in this gaf file
                    return null;
                }

                var frame = entry.Frames[0];

                Bitmap bmp;
                if (frame.Data == null || frame.Width == 0 || frame.Height == 0)
                {
                    bmp = new Bitmap(50, 50);
                }
                else
                {
                    bmp = BitmapConvert.ToBitmap(
                        frame.Data,
                        frame.Width,
                        frame.Height,
                        frame.TransparencyIndex);
                }

                return new OffsetBitmap(-frame.OffsetX, -frame.OffsetY, bmp);
        }

        private static GafEntry[] LoadGafEntries(HpiArchive.FileInfo fileInfo, HpiArchive archive)
        {
            // extract and read the file
            var fileBuffer = new byte[fileInfo.Size];
            archive.Extract(fileInfo, fileBuffer);
            var adapter = new GafEntryArrayAdapter();
            using (var b = new GafReader(new MemoryStream(fileBuffer), adapter))
            {
                b.Read();
            }

            var gaf = adapter.Entries;
            return gaf;
        }

        private IEnumerable<Feature> EnumerateFeaturesInternal(string world, string category)
        {
            var relevantFeatures = this.records
                .Where(
                    x => string.Equals(x.Value.World, world, StringComparison.InvariantCultureIgnoreCase)
                        && string.Equals(x.Value.Category, category, StringComparison.InvariantCultureIgnoreCase));

            return this.EnumerateFeaturesFromInternal(relevantFeatures);
        }

        private IEnumerable<Feature> EnumerateFeaturesFromInternal(IEnumerable<KeyValuePair<string, FeatureInfo>> relevantFeatures)
        {
            var uncachedFeatures = new List<KeyValuePair<string, FeatureInfo>>();
            foreach (var s in relevantFeatures)
            {
                if (this.featureCache.TryGetValue(s.Key, out var v))
                {
                    yield return v;
                }
                else
                {
                    uncachedFeatures.Add(s);
                }
            }

            var hpis = LoadingUtils.EnumerateSearchHpis().ToList();
            hpis.Reverse();

            if (uncachedFeatures.Count == 0)
            {
                yield break;
            }

            foreach (var hpi in hpis)
            {
                var nextUncachedFeatures = new List<KeyValuePair<string, FeatureInfo>>();
                var animFeatures = uncachedFeatures
                    .Where(x => !string.IsNullOrWhiteSpace(x.Value.AnimFileName))
                    .GroupBy(x => x.Value.AnimFileName);
                var objectFeatures = uncachedFeatures.Where(x => !string.IsNullOrWhiteSpace(x.Value.ObjectName));

                var archive = new HpiArchive(hpi);
                foreach (var entry in animFeatures)
                {
                    var fileInfo = archive.FindFile(HpiPath.Combine("anims", entry.Key + ".gaf"));
                    if (fileInfo == null)
                    {
                        nextUncachedFeatures.AddRange(entry);
                        continue;
                    }

                    var gaf = LoadGafEntries(fileInfo, archive);

                    foreach (var item in entry)
                    {
                        var bitmap = LoadBitmap(gaf, item.Value.SequenceName);
                        if (bitmap == null)
                        {
                            nextUncachedFeatures.Add(item);
                            continue;
                        }

                        var f = new Feature
                            {
                                Name = item.Value.Name,
                                World = item.Value.World,
                                Category = item.Value.Category,
                                Footprint = item.Value.Footprint,
                                Image = bitmap.Bitmap,
                                Offset = new Point(bitmap.OffsetX, bitmap.OffsetY)
                            };
                        this.featureCache.Add(item.Key, f);
                        yield return f;
                    }
                }

                foreach (var entry in objectFeatures)
                {
                    var objectFile = archive.FindFile(HpiPath.Combine("objects3d", entry.Value.ObjectName + ".3do"));
                    if (objectFile == null)
                    {
                        nextUncachedFeatures.Add(entry);
                        continue;
                    }

                    var bitmap = LoadRenderFile(archive, objectFile);

                    var f = new Feature
                        {
                            Name = entry.Value.Name,
                            World = entry.Value.World,
                            Category = entry.Value.Category,
                            Footprint = entry.Value.Footprint,
                            Image = bitmap.Bitmap,
                            Offset = new Point(bitmap.OffsetX, bitmap.OffsetY)
                        };
                    this.featureCache.Add(entry.Key, f);
                    yield return f;
                }

                uncachedFeatures = nextUncachedFeatures;
            }
        }

        private void AddFeature(FeatureInfo f)
        {
            this.records[f.Name] = f;
        }
    }
}
