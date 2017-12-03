namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Util;

    using TAUtil._3do;
    using TAUtil.Hpi;

    public class FeatureRenderLoader : AbstractHpiLoader<KeyValuePair<string, OffsetBitmap>>
    {
        private readonly IDictionary<string, IList<FeatureRecord>> objectMap;

        public FeatureRenderLoader(IDictionary<string, IList<FeatureRecord>> objectMap)
        {
            this.objectMap = objectMap;
        }

        protected override IEnumerable<HpiArchive.FileInfo> EnumerateFiles(HpiArchive r)
        {
            return r.GetFilesRecursive("objects3d")
                .Where(x =>
                    x.Name.EndsWith(".3do", StringComparison.OrdinalIgnoreCase)
                    && this.objectMap.ContainsKey(HpiPath.GetFileNameWithoutExtension(x.Name)));
        }

        protected override void LoadFile(HpiArchive archive, HpiArchive.FileInfo file)
        {
            var records = this.objectMap[HpiPath.GetFileNameWithoutExtension(file.Name)];

            var fileBuffer = new byte[file.Size];
            archive.Extract(file, fileBuffer);

            using (var b = new MemoryStream(fileBuffer))
            {
                var adapter = new ModelEdgeReaderAdapter();
                var reader = new ModelReader(b, adapter);
                reader.Read();
                var wire = Util.RenderWireframe(adapter.Edges);
                foreach (var record in records)
                {
                    this.Records.Add(new KeyValuePair<string, OffsetBitmap>(record.Name, wire));
                }
            }
        }
    }
}
