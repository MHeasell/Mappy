namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Palette;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    public class SectionLoader : AbstractHpiLoader<Section>
    {
        private readonly SectionFactory factory;

        public SectionLoader(IPalette palette)
        {
            this.factory = new SectionFactory(palette);
        }

        protected override void LoadFile(HpiReader h, string sect)
        {
            using (var s = new SctReader(h.ReadFile(sect)))
            {
                var section = new Section(h.FileName, sect);
                section.Name = HpiPath.GetFileNameWithoutExtension(sect);
                section.Minimap = this.factory.MinimapFromSct(s);
                section.DataWidth = s.DataWidth;
                section.DataHeight = s.DataHeight;

                string directoryString = HpiPath.GetDirectoryName(sect);
                Debug.Assert(directoryString != null, "Null directory for section in HPI.");
                string[] directories = directoryString.Split('\\');

                section.World = directories[1];
                section.Category = directories[2];

                this.Records.Add(section);
            }
        }

        protected override IEnumerable<string> EnumerateFiles(HpiReader r)
        {
            return r.GetFilesRecursive("sections")
                .Select(x => x.Name)
                .Where(x => x.EndsWith(".sct", StringComparison.OrdinalIgnoreCase));
        }
    }
}
