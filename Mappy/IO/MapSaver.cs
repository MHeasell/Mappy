namespace Mappy.IO
{
    using System.IO;

    using Mappy.Data;
    using Mappy.Models;

    using TAUtil.HpiUtil;
    using TAUtil.Tnt;

    /// <summary>
    /// Provides methods for writing out a IMapModel instance
    /// as TNT or HPI.
    /// </summary>
    public static class MapSaver
    {
        public static void SaveTnt(IReadOnlyMapModel map, string filename)
        {
            using (var s = new TntWriter(File.Create(filename)))
            {
                s.WriteTnt(new MapModelTntAdapter(map));
            }
        }

        public static void SaveOta(MapAttributes attrs, string filename)
        {
            using (Stream s = File.Create(filename))
            {
                attrs.WriteOta(s);
            }
        }

        public static void SaveHpi(IReadOnlyMapModel map, string filename)
        {
            var namePart = Path.GetFileNameWithoutExtension(filename);

            var tmpTntName = Path.GetTempFileName();
            var tmpOtaName = Path.GetTempFileName();

            try
            {
                using (var s = new TntWriter(File.Create(tmpTntName)))
                {
                    s.WriteTnt(new MapModelTntAdapter(map));
                }

                using (Stream s = File.Create(tmpOtaName))
                {
                    map.Attributes.WriteOta(s);
                }

                var fname = "maps\\" + namePart;

                using (var wr = new HpiWriter(filename, HpiWriter.CompressionMethod.ZLib))
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
