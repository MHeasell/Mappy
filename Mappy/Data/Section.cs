namespace Mappy.Data
{
    using System.Drawing;

    /// <summary>
    /// Contains info about a loaded section, various metadata.
    /// </summary>
    public class Section
    {
        public Section(string hapiPath, string path)
        {
            this.HpiFileName = hapiPath;
            this.SctFileName = path;
        }

        public string World { get; set; }

        public string Category { get; set; }

        public string Name { get; set; }

        public int DataWidth { get; set; }

        public int DataHeight { get; set; }

        public int PixelWidth => this.DataWidth * 32;

        public int PixelHeight => this.DataHeight * 32;

        public Bitmap Minimap { get; set; }

        public string HpiFileName { get; }

        public string SctFileName { get; }
    }
}
