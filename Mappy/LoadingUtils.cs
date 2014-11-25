namespace Mappy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Data;

    using Mappy.IO;
    using Mappy.Palette;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    public static class LoadingUtils
    {
        public static IEnumerable<string> EnumerateSearchHpis()
        {
            string[] exts = { "hpi", "ufo", "ccx", "gpf", "gp3" };
            if (MappySettings.Settings.SearchPaths == null)
            {
                yield break;
            }

            foreach (string ext in exts)
            {
                foreach (string dir in MappySettings.Settings.SearchPaths)
                {
                    if (!Directory.Exists(dir))
                    {
                        // silently ignore missing directories
                        continue;
                    }

                    foreach (string file in Directory.EnumerateFiles(dir, "*." + ext))
                    {
                        yield return file;
                    }
                }
            }
        }

        public static IList<Section> LoadSections(IPalette palette)
        {
            IList<Section> sections = new List<Section>();
            int i = 0;
            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                foreach (Section s in LoadingUtils.LoadSectionsFromHapi(file, palette))
                {
                    s.Id = i++;
                    sections.Add(s);
                }
            }

            return sections;
        }

        private static IEnumerable<Section> LoadSectionsFromHapi(string filename, IPalette palette)
        {
            var factory = new SectionFactory(palette);

            using (HpiReader h = new HpiReader(filename))
            {
                foreach (string sect in h.GetFilesRecursive("sections").Select(x => x.Name))
                {
                    using (var s = new SctReader(h.ReadFile(sect)))
                    {
                        Section section = new Section(filename, sect);
                        section.Name = Path.GetFileNameWithoutExtension(sect);
                        section.Minimap = factory.MinimapFromSct(s);
                        section.DataWidth = s.DataWidth;
                        section.DataHeight = s.DataHeight;

                        string[] directories = Path.GetDirectoryName(sect).Split(Path.DirectorySeparatorChar);

                        section.World = directories[1];
                        section.Category = directories[2];

                        yield return section;
                    }
                }
            }
        }
    }
}
