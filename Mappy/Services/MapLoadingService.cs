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
        private readonly SectionFactory sectionFactory = new SectionFactory();

        private readonly MapModelFactory mapModelFactory = new MapModelFactory();

        public UndoableMapModel CreateMap(int width, int height)
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

        public UndoableMapModel CreateFromHpi(string hpipath, string mappath, bool readOnly = false)
        {
            MapModel m;

            using (HpiReader hpi = new HpiReader(hpipath))
            {
                string otaPath = HpiPath.ChangeExtension(mappath, ".ota");

                TdfNode n;

                using (var ota = hpi.ReadTextFile(otaPath))
                {
                    n = TdfNode.LoadTdf(ota);
                }

                using (var s = new TntReader(hpi.ReadFile(mappath)))
                {
                    m = this.mapModelFactory.FromTntAndOta(s, n);
                }
            }

            return new UndoableMapModel(m, hpipath, readOnly);
        }
    }
}
