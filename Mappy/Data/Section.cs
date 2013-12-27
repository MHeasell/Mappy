namespace Mappy.Data
{
    using System.Drawing;
    using System.IO;

    using Mappy.IO;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    public class Section
    {
        private MapTile cachedTile;
        private string hapiPath;
        private string cachedTilePath;

        public Section(string hapiPath, string path)
        {
            this.hapiPath = hapiPath;
            this.cachedTilePath = path;
        }

        public int Id { get; set; }

        public string World { get; set; }

        public string Category { get; set; }

        public string Name { get; set; }

        public Bitmap Minimap { get; set; }

        public MapTile GetTile()
        {
            if (this.cachedTile == null)
            {
                this.LoadTile();
            }

            return this.cachedTile;
        }

        private void LoadTile()
        {
            string outpath = Path.GetTempFileName();

            using (HpiReader h = new HpiReader(this.hapiPath))
            {
                h.ExtractFile(this.cachedTilePath, outpath);
            }

            using (var s = new SctReader(File.OpenRead(outpath)))
            {
                SectionFactory factory = new SectionFactory(Globals.Palette);
                this.cachedTile = factory.TileFromSct(s);
            }

            File.Delete(outpath);
        }
    }
}
