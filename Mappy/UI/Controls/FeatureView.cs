namespace Mappy.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Data;

    public partial class FeatureView : UserControl
    {
        private IList<Feature> features;

        private IList<TabControl> worldControls = new List<TabControl>();

        public FeatureView()
        {
            this.InitializeComponent();
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
            this.panel1.Controls.Clear();
            this.comboBox1.Items.Clear();
            
            if (this.Features == null)
            {
                return;
            }

            foreach (var world in this.Features.GroupBy(x => x.World))
            {
                this.comboBox1.Items.Add(world.Key);

                TabControl tabs = new TabControl();
                tabs.Dock = DockStyle.Fill;

                foreach (var group in world.GroupBy(x => x.Category))
                {
                    ListView view = this.CreateViewFor(group);
                    view.MultiSelect = false;
                    view.Dock = DockStyle.Fill;
                    view.ItemDrag += this.ListView_ItemDrag;
                    
                    TabPage page = new TabPage(group.Key);
                    page.Controls.Add(view);
                    tabs.Controls.Add(page);
                }

                this.worldControls.Add(tabs);

                if (this.comboBox1.Items.Count > 0)
                {
                    this.comboBox1.SelectedIndex = 0;
                }
            }
        }

        private ListView CreateViewFor(IEnumerable<Feature> features)
        {
            ListView view = new ListView();

            ImageList imgs = new ImageList();
            imgs.ImageSize = new Size(64, 64);
            foreach (Feature f in features)
            {
                imgs.Images.Add(this.RescaleImage(f.Image));
            }

            view.LargeImageList = imgs;

            int i = 0;
            foreach (Feature f in features)
            {
                ListViewItem item = new ListViewItem(f.Name, i++);
                item.Tag = f;
                view.Items.Add(item);
            }

            return view;
        }

        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListView view = sender as ListView;

            if (view.SelectedIndices.Count == 0)
            {
                return;
            }

            string name = ((Feature)view.SelectedItems[0].Tag).Name;
            view.DoDragDrop(name, DragDropEffects.Copy);
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(this.worldControls[this.comboBox1.SelectedIndex]);
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
