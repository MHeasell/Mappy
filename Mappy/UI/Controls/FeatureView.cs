namespace Mappy.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using Mappy.Data;

    public partial class FeatureView : UserControl
    {
        private IList<Feature> features;

        private readonly Dictionary<string, Bitmap> rescaledImageMap = new Dictionary<string, Bitmap>();

        public FeatureView()
        {
            this.InitializeComponent();

            this.control.comboBox1.SelectedIndexChanged += this.ComboBox1SelectedIndexChanged;
            this.control.comboBox2.SelectedIndexChanged += this.ComboBox2SelectedIndexChanged;
            this.control.listView1.ItemDrag += this.ListViewItemDrag;
        }

        public IList<Feature> Features
        {
            get
            {
                return this.features;
            }

            set
            {
                this.features = value;
                this.PopulateView();
            }
        }

        private void PopulateView()
        {
            this.control.comboBox1.Items.Clear();
            this.control.comboBox2.Items.Clear();
            this.control.listView1.Items.Clear();

            if (this.Features == null)
            {
                return;
            }

            this.PopulateWorldsComboBox();
        }

        private void PopulateWorldsComboBox()
        {
            this.control.comboBox1.Items.Clear();

            var worlds = this.Features.Select(x => x.World)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

            foreach (var world in worlds)
            {
                this.control.comboBox1.Items.Add(world);
            }

            if (this.control.comboBox1.Items.Count > 0)
            {
                this.control.comboBox1.SelectedIndex = 0;
            }
        }

        private void PopulateCategoryComboBox()
        {
            this.control.comboBox2.Items.Clear();

            var world = (string)this.control.comboBox1.SelectedItem;

            var categories = this.Features.Where(x => x.World == world)
                .Select(x => x.Category)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

            foreach (var category in categories)
            {
                this.control.comboBox2.Items.Add(category);
            }

            if (this.control.comboBox2.Items.Count > 0)
            {
                this.control.comboBox2.SelectedIndex = 0;
            }
        }

        private void PopulateListView()
        {
            this.control.listView1.Items.Clear();

            var world = (string)this.control.comboBox1.SelectedItem;
            var category = (string)this.control.comboBox2.SelectedItem;

            var features = this.Features
                .Where(x => x.World == world && x.Category == category)
                .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            var images = new ImageList();
            images.ImageSize = new Size(64, 64);
            foreach (var f in features)
            {
                images.Images.Add(this.GetRescaledImage(f.Name, f.Image));
            }

            this.control.listView1.LargeImageList = images;

            var i = 0;
            foreach (var f in features)
            {
                var item = new ListViewItem(f.Name, i++) { Tag = f };
                this.control.listView1.Items.Add(item);
            }
        }

        private void ListViewItemDrag(object sender, ItemDragEventArgs e)
        {
            var view = (ListView)sender;

            if (view.SelectedIndices.Count == 0)
            {
                return;
            }

            string name = ((Feature)view.SelectedItems[0].Tag).Name;
            view.DoDragDrop(name, DragDropEffects.Copy);
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopulateCategoryComboBox();
        }

        private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopulateListView();
        }

        private Bitmap GetRescaledImage(string name, Bitmap img)
        {
            Bitmap rescaledImage;
            if (!this.rescaledImageMap.TryGetValue(name, out rescaledImage))
            {
                rescaledImage = this.RescaleImage(img);
                this.rescaledImageMap[name] = rescaledImage;
            }

            return rescaledImage;
        }

        private Bitmap RescaleImage(Bitmap img)
        {
            int outWidth = 64;
            int outHeight = 64;

            Bitmap thumb = new Bitmap(outWidth, outHeight);
            Graphics g = Graphics.FromImage(thumb);

            double ratioX = outWidth / (double)img.Width;
            double ratioY = outHeight / (double)img.Height;

            // use the smaller ratio
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth;
            int newHeight;
            if (img.Width <= outWidth && img.Height <= outHeight)
            {
                // keep original image size if smaller
                newWidth = img.Width;
                newHeight = img.Height;
            }
            else
            {
                newWidth = (int)(img.Width * ratio);
                newHeight = (int)(img.Height * ratio);
            }

            int posX = (outWidth - newWidth) / 2;
            int posY = (outHeight - newHeight) / 2;

            g.DrawImage(img, posX, posY, newWidth, newHeight);

            return thumb;
        }
    }
}
