namespace Mappy.Data
{
    using System.Drawing;
    using System.IO;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    public class Section
    {
        private MapTile cachedTile;
        private string hapiPath;
        private string cachedTilePath;
        private Color[] palette;

        public Section(string hapiPath, string path, Color[] palette)
        {
            this.hapiPath = hapiPath;
            this.cachedTilePath = path;
            this.palette = palette;
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
            string tmpdir = System.Environment.GetEnvironmentVariable("TEMP");
            string outpath = Path.Combine(tmpdir, this.cachedTilePath);

            if (!File.Exists(outpath))
            {
                using (HpiReader h = new HpiReader(this.hapiPath))
                {
                    h.ExtractFile(this.cachedTilePath, outpath);
                }
            }

            using (Stream s = File.OpenRead(outpath))
            {
                this.cachedTile = MapTile.ReadFromSct(new SctFile(s), this.palette);
            }
        }
    }
}
