namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Mappy.Data;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    public class SectionLoader : AbstractHpiLoader<Section>
    {
        private readonly SectionFactory factory;

        public SectionLoader()
        {
            this.factory = new SectionFactory();
        }

        protected override void LoadFile(HpiEntry file)
        {
            using (var s = new SctReader(file.Open()))
            {
                var section = new Section(file.Reader.FileName, file.Name);
                section.Name = HpiPath.GetFileNameWithoutExtension(file.Name);
                section.Minimap = this.factory.MinimapFromSct(s);
                section.DataWidth = s.DataWidth;
                section.DataHeight = s.DataHeight;

                string directoryString = HpiPath.GetDirectoryName(file.Name);
                Debug.Assert(directoryString != null, "Null directory for section in HPI.");
                string[] directories = directoryString.Split('\\');

                section.World = directories[1];
                section.Category = directories[2];

                this.Records.Add(section);
            }
        }

        protected override IEnumerable<HpiEntry> EnumerateFiles(HpiReader r)
        {
            return r.GetFilesRecursive("sections")
                .Where(x => x.Name.EndsWith(".sct", StringComparison.OrdinalIgnoreCase));
        }
    }
}
