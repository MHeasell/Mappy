namespace Mappy.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mappy.Data;

    public class SectionService
    {
        private readonly Dictionary<int, Section> sections = new Dictionary<int, Section>();

        private int nextId;

        public event EventHandler SectionsChanged;

        public Section Get(int id) => this.sections[id];

        public void AddSections(IEnumerable<Section> sections)
        {
            foreach (var s in sections)
            {
                this.AddSection(s);
            }

            this.SectionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<Section> EnumerateAll() => this.sections.Select(x => x.Value);

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

        public IEnumerable<KeyValuePair<int, Section>> EnumerateSections(string world, string category) =>
            this.sections
                .Where(x => string.Equals(x.Value.World, world, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(x.Value.Category, category, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(x => x.Value.Name, StringComparer.InvariantCultureIgnoreCase);

        private void AddSection(Section s)
        {
            var id = this.nextId++;
            this.sections[id] = s;
        }
    }
}
