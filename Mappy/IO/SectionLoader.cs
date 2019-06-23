namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Mappy.Data;

    using TAUtil.Hpi;

    public class SectionLoader : AbstractHpiLoader<SectionInfo>
    {
        protected override void LoadFile(HpiArchive archive, HpiArchive.FileInfo file)
        {
            var directoryString = HpiPath.GetDirectoryName(file.FullPath);
            Debug.Assert(directoryString != null, "Null directory for section in HPI.");
            var directories = directoryString.Split('\\');

            this.Records.Add(new SectionInfo
            {
                HpiFileName = archive.FileName,
                SctFileName = file.FullPath,
                World = directories[1],
                Category = directories[2]
            });
        }

        protected override IEnumerable<HpiArchive.FileInfo> EnumerateFiles(HpiArchive r)
        {
            return r.GetFilesRecursive("sections")
                .Where(x => x.Name.EndsWith(".sct", StringComparison.OrdinalIgnoreCase));
        }
    }
}
