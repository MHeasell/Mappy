namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mappy.Data;

    using TAUtil.Hpi;
    using TAUtil.Tdf;

    public class FeatureTdfLoader : AbstractHpiLoader<FeatureRecord>
    {
        protected override void LoadFile(HpiReader r, string file)
        {
            TdfNode n;
            using (var tdf = r.ReadTextFile(file))
            {
                n = TdfNode.LoadTdf(tdf);
            }

            this.Records.AddRange(
                n.Keys.Values.Select(FeatureRecord.FromTdfNode));
        }

        protected override IEnumerable<string> EnumerateFiles(HpiReader r)
        {
            return r.GetFilesRecursive("features")
                .Select(x => x.Name)
                .Where(x => x.EndsWith(".tdf", StringComparison.OrdinalIgnoreCase));
        }
    }
}
