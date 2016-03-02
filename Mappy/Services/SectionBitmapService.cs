namespace Mappy.Services
{
    using System.Collections.Generic;
    using System.IO;

    using Mappy.Data;
    using Mappy.IO;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    public class SectionBitmapService
    {
        private readonly SectionFactory sectionFactory;

        private readonly Dictionary<string, MapTile> tileCache = new Dictionary<string, MapTile>();

        public SectionBitmapService(SectionFactory sectionFactory)
        {
            this.sectionFactory = sectionFactory;
        }

        public MapTile LoadSection(string hpiFileName, string sctFileName)
        {
            var key = HpiPath.Combine(hpiFileName, sctFileName);

            MapTile section;
            if (!this.tileCache.TryGetValue(key, out section))
            {
                section = this.LoadSectionFromDisk(hpiFileName, sctFileName);
                this.tileCache[key] = section;
            }

            return section;
        }

        private MapTile LoadSectionFromDisk(string hpiFileName, string sctFileName)
        {
            var outpath = Path.GetTempFileName();

            try
            {
                using (var h = new HpiReader(hpiFileName))
                {
                    h.ExtractFile(sctFileName, outpath);
                }

                using (var s = new SctReader(File.OpenRead(outpath)))
                {
                    return this.sectionFactory.TileFromSct(s);
                }
            }
            finally
            {
                File.Delete(outpath);
            }
        }
    }
}
