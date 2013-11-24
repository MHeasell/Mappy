namespace Mappy.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Data;
    using Mappy.Models;

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

            foreach (var world in this.Sections.GroupBy(x => x.World))
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
            }

            if (this.comboBox1.Items.Count > 0)
            {
                this.comboBox1.SelectedIndex = 0;
            }
        }

        private ListView CreateViewFor(IEnumerable<Section> sections)
        {
            ListView listView = new ListView();

            ImageList imgs = new ImageList();
            imgs.ImageSize = new Size(128, 128);
            foreach (Section s in sections)
            {
                imgs.Images.Add(s.Minimap);
            }

            listView.LargeImageList = imgs;

            int i = 0;
            foreach (Section s in sections)
            {
                ListViewItem item = new ListViewItem(s.Name, i++);
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
