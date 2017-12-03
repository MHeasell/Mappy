namespace Mappy.Services
{
    using System.IO;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.IO;
    using Mappy.Models;

    using TAUtil.Hpi;
    using TAUtil.Sct;
    using TAUtil.Tdf;
    using TAUtil.Tnt;

    public class MapLoadingService
    {
        private readonly SectionFactory sectionFactory;

        private readonly MapModelFactory mapModelFactory;

        public MapLoadingService(SectionFactory sectionFactory, MapModelFactory mapModelFactory)
        {
            this.sectionFactory = sectionFactory;
            this.mapModelFactory = mapModelFactory;
        }

        public static UndoableMapModel CreateMap(int width, int height)
        {
            var map = new MapModel(width, height);
            GridMethods.Fill(map.Tile.TileGrid, Globals.DefaultTile);
            return new UndoableMapModel(map, null, false);
        }

        public UndoableMapModel CreateFromSct(string filename)
        {
            MapTile t;
            using (var s = new SctReader(filename))
            {
                t = this.sectionFactory.TileFromSct(s);
            }

            return new UndoableMapModel(new MapModel(t), filename, true);
        }

        public UndoableMapModel CreateFromTnt(string filename)
        {
            MapModel m;

            var otaFileName = filename.Substring(0, filename.Length - 4) + ".ota";
            if (File.Exists(otaFileName))
            {
                TdfNode attrs;
                using (var ota = File.OpenRead(otaFileName))
                {
                    attrs = TdfNode.LoadTdf(ota);
                }

                using (var s = new TntReader(filename))
                {
                    m = this.mapModelFactory.FromTntAndOta(s, attrs);
                }
            }
            else
            {
                using (var s = new TntReader(filename))
                {
                    m = this.mapModelFactory.FromTnt(s);
                }
            }

            return new UndoableMapModel(m, filename, false);
        }

        public UndoableMapModel CreateFromHpi(string hpipath, string mappath)
        {
            return this.CreateFromHpi(hpipath, mappath, false);
        }

        public UndoableMapModel CreateFromHpi(string hpipath, string mappath, bool readOnly)
        {
            MapModel m;

            using (var hpi = new HpiArchive(hpipath))
            {
                string otaPath = HpiPath.ChangeExtension(mappath, ".ota");

                TdfNode n;

                var otaFileInfo = hpi.FindFile(otaPath);
                var otaFileBuffer = new byte[otaFileInfo.Size];
                hpi.Extract(otaFileInfo, otaFileBuffer);
                using (var ota = new MemoryStream(otaFileBuffer))
                {
                    n = TdfNode.LoadTdf(ota);
                }

                var tntFileInfo = hpi.FindFile(mappath);
                var tntFileBuffer = new byte[tntFileInfo.Size];
                hpi.Extract(tntFileInfo, tntFileBuffer);
                using (var s = new TntReader(new MemoryStream(tntFileBuffer)))
                {
                    m = this.mapModelFactory.FromTntAndOta(s, n);
                }
            }

            return new UndoableMapModel(m, hpipath, readOnly);
        }
    }
}
