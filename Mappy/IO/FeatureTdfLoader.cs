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
        protected override void LoadFile(HpiEntry file)
        {
            TdfNode n;
            using (var tdf = file.Open())
            {
                n = TdfNode.LoadTdf(tdf);
            }

            this.Records.AddRange(
                n.Keys.Values.Select(FeatureRecord.FromTdfNode));
        }

        protected override IEnumerable<HpiEntry> EnumerateFiles(HpiReader r)
        {
            return r.GetFilesRecursive("features")
                .Where(x => x.Name.EndsWith(".tdf", StringComparison.OrdinalIgnoreCase));
        }
    }
}
