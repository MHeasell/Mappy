namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Util;

    using TAUtil.Hpi;
    using TAUtil._3do;

    public class FeatureRenderLoader : AbstractHpiLoader<KeyValuePair<string, OffsetBitmap>>
    {
        private readonly IDictionary<string, IList<FeatureRecord>> objectMap;

        public FeatureRenderLoader(IDictionary<string, IList<FeatureRecord>> objectMap)
        {
            this.objectMap = objectMap;
        }

        protected override IEnumerable<string> EnumerateFiles(HpiReader r)
        {
            return r.GetFilesRecursive("objects3d")
                .Select(x => x.Name)
                .Where(x =>
                    x.EndsWith(".3do", StringComparison.OrdinalIgnoreCase)
                    && this.objectMap.ContainsKey(HpiPath.GetFileNameWithoutExtension(x)));
        }

        protected override void LoadFile(HpiReader r, string file)
        {
            Debug.Assert(file != null, "Null filename");
            var records = this.objectMap[HpiPath.GetFileNameWithoutExtension(file)];

            using (var b = r.ReadFile(file))
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
