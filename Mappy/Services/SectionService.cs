namespace Mappy.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Mappy.Data;
    using Mappy.IO;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    public class SectionService
    {
        private readonly Dictionary<int, SectionInfo> sections = new Dictionary<int, SectionInfo>();

        private readonly Dictionary<int, Section> sectionsCache = new Dictionary<int, Section>();

        private int nextId;

        public event EventHandler SectionsChanged;

        public SectionInfo Get(int id) => this.sections[id];

        public void AddSections(IEnumerable<SectionInfo> sections)
        {
            foreach (var s in sections)
            {
                this.AddSection(s);
            }

            this.SectionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<string> EnumerateWorlds() =>
            this.sections
                .Select(x => x.Value.World)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<string> EnumerateCategories(string world) =>
            this.sections
                .Select(x => x.Value)
                .Where(x => x.World == world)
                .Select(x => x.Category)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<KeyValuePair<int, Section>> EnumerateSections(string world, string category)
        {
            return this.EnumerateSectionsInternal(world, category).OrderBy(
                x => x.Value.Name,
                StringComparer.InvariantCultureIgnoreCase);
        }

        private static Section LoadSection(HpiArchive archive, HpiArchive.FileInfo fileInfo)
        {
            var fileBuffer = new byte[fileInfo.Size];
            archive.Extract(fileInfo, fileBuffer);
            using (var s = new SctReader(new MemoryStream(fileBuffer)))
            {
                var section = new Section(archive.FileName, fileInfo.FullPath);
                section.Name = HpiPath.GetFileNameWithoutExtension(fileInfo.Name);
                section.Minimap = SectionFactory.MinimapFromSct(s);
                section.DataWidth = s.DataWidth;
                section.DataHeight = s.DataHeight;

                var directoryString = HpiPath.GetDirectoryName(fileInfo.FullPath);
                Debug.Assert(directoryString != null, "Null directory for section in HPI.");
                var directories = directoryString.Split('\\');
                section.World = directories[1];
                section.Category = directories[2];

                return section;
            }
        }

        private IEnumerable<KeyValuePair<int, Section>> EnumerateSectionsInternal(string world, string category)
        {
            var relevantSections = this.sections.Where(
                x => string.Equals(x.Value.World, world, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(x.Value.Category, category, StringComparison.InvariantCultureIgnoreCase));

            var uncachedSections = new List<KeyValuePair<int, SectionInfo>>();
            foreach (var s in relevantSections)
            {
                if (this.sectionsCache.TryGetValue(s.Key, out var v))
                {
                    yield return new KeyValuePair<int, Section>(s.Key, v);
                }
                else
                {
                    uncachedSections.Add(s);
                }
            }

            foreach (var entry in uncachedSections.GroupBy(x => x.Value.HpiFileName))
            {
                var archive = new HpiArchive(entry.Key);
                foreach (var item in entry)
                {
                    var fileInfo = archive.FindFile(item.Value.SctFileName);
                    if (fileInfo != null)
                    {
                        var section = LoadSection(archive, fileInfo);
                        this.sectionsCache.Add(item.Key, section);
                        yield return new KeyValuePair<int, Section>(item.Key, section);
                    }
                }
            }
        }

        private void AddSection(SectionInfo s)
        {
            var id = this.nextId++;
            this.sections[id] = s;
        }
    }
}
