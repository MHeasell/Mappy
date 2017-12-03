namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Mappy.Data;

    using TAUtil.Hpi;
    using TAUtil.Tdf;

    public class FeatureTdfLoader : AbstractHpiLoader<FeatureRecord>
    {
        protected override void LoadFile(HpiArchive archive, HpiArchive.FileInfo file)
        {
            var fileBuffer = new byte[file.Size];
            archive.Extract(file, fileBuffer);

            TdfNode n;
            using (var tdf = new MemoryStream(fileBuffer))
            {
                n = TdfNode.LoadTdf(tdf);
            }

            this.Records.AddRange(
                n.Keys.Values.Select(FeatureRecord.FromTdfNode));
        }

        protected override IEnumerable<HpiArchive.FileInfo> EnumerateFiles(HpiArchive r)
        {
            return r.GetFilesRecursive("features")
                .Where(x => x.Name.EndsWith(".tdf", StringComparison.OrdinalIgnoreCase));
        }
    }
}
