namespace Mappy.Data
{
    using System.Drawing;
    using System.IO;

    using Mappy.IO;

    using TAUtil.Hpi;
    using TAUtil.Sct;

    /// <summary>
    /// Contains info about a loaded section, various metadata.
    /// Also supports lazy-loading of the section bitmap.
    /// </summary>
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

        public string World { get; set; }

        public string Category { get; set; }

        public string Name { get; set; }

        public int DataWidth { get; set; }

        public int DataHeight { get; set; }

        public int PixelWidth => this.DataWidth * 32;

        public int PixelHeight => this.DataHeight * 32;

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
                this.cachedTile = SectionFactory.TileFromSct(s);
            }

            File.Delete(outpath);
        }
    }
}
