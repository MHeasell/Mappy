namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Mappy.Data;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    public class SectionLoader : AbstractHpiLoader<Section>
    {
        protected override void LoadFile(HpiArchive archive, HpiArchive.FileInfo file)
        {
            var fileBuffer = new byte[file.Size];
            archive.Extract(file, fileBuffer);

            using (var s = new SctReader(new MemoryStream(fileBuffer)))
            {
                var section = new Section(archive.FileName, file.FullPath);
                section.Name = HpiPath.GetFileNameWithoutExtension(file.Name);
                section.Minimap = SectionFactory.MinimapFromSct(s);
                section.DataWidth = s.DataWidth;
                section.DataHeight = s.DataHeight;

                string directoryString = HpiPath.GetDirectoryName(file.FullPath);
                Debug.Assert(directoryString != null, "Null directory for section in HPI.");
                string[] directories = directoryString.Split('\\');

                section.World = directories[1];
                section.Category = directories[2];

                this.Records.Add(section);
            }
        }

        protected override IEnumerable<HpiArchive.FileInfo> EnumerateFiles(HpiArchive r)
        {
            return r.GetFilesRecursive("sections")
                .Where(x => x.Name.EndsWith(".sct", StringComparison.OrdinalIgnoreCase));
        }
    }
}
