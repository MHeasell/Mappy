namespace Mappy.IO
{
    using System.IO;

    using Mappy.Models;
    using Mappy.Palette;

    using TAUtil.Hpi;
    using TAUtil.Tnt;

    /// <summary>
    /// Provides methods for writing out a IMapModel instance
    /// as TNT or HPI.
    /// </summary>
    public class MapSaver
    {
        private readonly IReversePalette reversePalette;

        public MapSaver(IReversePalette reversePalette)
        {
            this.reversePalette = reversePalette;
        }

        public void SaveTnt(IMapModel map, string filename)
        {
            using (var s = new TntWriter(File.Create(filename)))
            {
                s.WriteTnt(new MapModelTntAdapter(map, this.reversePalette));
            }
        }

        public void SaveHpi(IMapModel map, string filename)
        {
            string tmpTntName = Path.GetTempFileName();
            string tmpOtaName = Path.GetTempFileName();

            try
            {
                using (var s = new TntWriter(File.Create(tmpTntName)))
                {
                    s.WriteTnt(new MapModelTntAdapter(map, this.reversePalette));
                }

                using (Stream s = File.Create(tmpOtaName))
                {
                    map.Attributes.WriteOta(s);
                }

                string fname = "maps\\" + map.Attributes.Name;

                using (HpiWriter wr = new HpiWriter(filename, HpiWriter.CompressionMethod.ZLib))
                {
                    wr.AddFile(fname + ".tnt", tmpTntName);
                    wr.AddFile(fname + ".ota", tmpOtaName);
                }
            }
            finally
            {
                if (File.Exists(tmpTntName))
                {
                    File.Delete(tmpTntName);
                }

                if (File.Exists(tmpOtaName))
                {
                    File.Delete(tmpOtaName);
                }
            }
        }
    }
}
