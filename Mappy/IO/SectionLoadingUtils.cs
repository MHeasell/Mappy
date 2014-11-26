namespace Mappy.IO
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Palette;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    public class SectionLoadingUtils
    {
        public static BackgroundWorker LoadSectionsBackgroundWorker()
        {
            var bg = new BackgroundWorker();

            bg.DoWork += delegate(object sender, DoWorkEventArgs args)
                {
                    var worker = (BackgroundWorker)sender;
                    var palette = (IPalette)args.Argument;

                    IList<Section> sections = new List<Section>();
                    var hpis = LoadingUtils.EnumerateSearchHpis().ToList();

                    int i = 0;
                    int fileCount = 0;
                    foreach (string file in hpis)
                    {
                        foreach (Section s in LoadSectionsFromHapi(file, palette))
                        {
                            if (worker.CancellationPending)
                            {
                                args.Cancel = true;
                                return;
                            }

                            s.Id = i++;
                            sections.Add(s);
                        }

                        worker.ReportProgress((++fileCount * 100) / hpis.Count);
                    }

                    args.Result = sections;
                };

            bg.WorkerSupportsCancellation = true;
            bg.WorkerReportsProgress = true;

            return bg;
        }

        public static IList<Section> LoadSections(IPalette palette)
        {
            IList<Section> sections = new List<Section>();
            int i = 0;
            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                foreach (Section s in LoadSectionsFromHapi(file, palette))
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

                        string directoryString = Path.GetDirectoryName(sect);
                        Debug.Assert(directoryString != null, "Null directory for section in HPI.");
                        string[] directories = directoryString.Split(Path.DirectorySeparatorChar);

                        section.World = directories[1];
                        section.Category = directories[2];

                        yield return section;
                    }
                }
            }
        }
    }
}
