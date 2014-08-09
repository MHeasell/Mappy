namespace Mappy.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Data;

    public partial class SectionView : UserControl
    {
        private IList<Section> sections;

        private IList<TabControl> worldControls = new List<TabControl>();

        public SectionView()
        {
            this.InitializeComponent();
        }

        public IList<Section> Sections
        {
            get
            {
                return this.sections;
            }

            set
            {
                this.sections = value;
                this.PopulateView();
            }
        }

        private void PopulateView()
        {
            this.panel1.Controls.Clear();
            this.comboBox1.Items.Clear();

            if (this.Sections == null)
            {
                return;
            }

            foreach (var world in this.Sections.GroupBy(x => x.World.ToLowerInvariant()).OrderBy(x => x.Key))
            {
                this.comboBox1.Items.Add(world.Key);

                TabControl tabs = new TabControl();
                tabs.Dock = DockStyle.Fill;

                foreach (var group in world.GroupBy(x => x.Category.ToLowerInvariant()).OrderBy(x => x.Key))
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
            }

            if (this.comboBox1.Items.Count > 0)
            {
                this.comboBox1.SelectedIndex = 0;
            }
        }

        private ListView CreateViewFor(IEnumerable<Section> sections)
        {
            var sList = sections.OrderBy(x => x.Name).ToList();

            ListView listView = new ListView();

            ImageList imgs = new ImageList();
            imgs.ImageSize = new Size(128, 128);
            foreach (Section s in sList)
            {
                imgs.Images.Add(s.Minimap);
            }

            listView.LargeImageList = imgs;

            int i = 0;
            foreach (Section s in sList)
            {
                var label = string.Format("{0} ({1}x{2})", s.Name, s.PixelWidth, s.PixelHeight);
                ListViewItem item = new ListViewItem(label, i++);
                item.Tag = s;
                listView.Items.Add(item);
            }

            return listView;
        }

        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListView view = sender as ListView;

            if (view.SelectedIndices.Count == 0)
            {
                return;
            }

            int id = ((Section)view.SelectedItems[0].Tag).Id;
            view.DoDragDrop(id.ToString(), DragDropEffects.Copy);
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(this.worldControls[this.comboBox1.SelectedIndex]);
        }
    }
}
