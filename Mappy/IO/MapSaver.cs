namespace Mappy.IO
{
    using System;
    using System.IO;

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

        public static void SaveOta(IReadOnlyMapModel map, string filename)
        {
            var mapDimensions = ComputeMapDimensions512(map);
            using (Stream s = File.Create(filename))
            {
                map.Attributes.WriteOta(s, mapDimensions.Item1, mapDimensions.Item2);
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

                var mapDimensions = ComputeMapDimensions512(map);

                using (Stream s = File.Create(tmpOtaName))
                {
                    map.Attributes.WriteOta(s, mapDimensions.Item1, mapDimensions.Item2);
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

        private static (int, int) ComputeMapDimensions512(IReadOnlyMapModel map)
        {
            var w = (map.FeatureGridWidth / 32) + ((map.FeatureGridWidth % 32 == 0) ? 0 : 1);
            var h = (map.FeatureGridHeight / 32) + ((map.FeatureGridHeight % 32 == 0) ? 0 : 1);
            return (w, h);
        }
    }
}
