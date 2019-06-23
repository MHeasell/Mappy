namespace Mappy
{
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Xml.Serialization;

    public class Configuration
    {
        [XmlIgnore]
        public Color GridColor
        {
            get => Color.FromArgb(this.GridColorArgb);
            set => this.GridColorArgb = value.ToArgb();
        }

        [XmlElement(ElementName = "GridColor")]
        public int GridColorArgb { get; set; }

        public StringCollection SearchPaths { get; set; }
    }
}
