namespace Mappy.Models
{
    using System.Drawing;

    public struct ListViewItem
    {
        public ListViewItem(string name, Bitmap image, object tag)
        {
            this.Name = name;
            this.Image = image;
            this.Tag = tag;
        }

        public string Name { get; }

        public Bitmap Image { get; }

        public object Tag { get; }
    }
}