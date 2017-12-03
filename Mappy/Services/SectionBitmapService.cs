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
                byte[] fileBuffer;
                using (var h = new HpiArchive(hpiFileName))
                {
                    var fileInfo = h.FindFile(sctFileName);
                    fileBuffer = new byte[fileInfo.Size];
                    h.Extract(fileInfo, fileBuffer);
                }

                using (var s = new SctReader(new MemoryStream(fileBuffer)))
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
