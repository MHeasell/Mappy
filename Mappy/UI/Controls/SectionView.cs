namespace Mappy.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using Mappy.Data;

    public partial class SectionView : UserControl
    {
        private IList<Section> sections;

        public SectionView()
        {
            this.InitializeComponent();

            this.control.comboBox1.SelectedIndexChanged += this.ComboBox1SelectedIndexChanged;
            this.control.comboBox2.SelectedIndexChanged += this.ComboBox2SelectedIndexChanged;
            this.control.listView1.ItemDrag += this.ListViewItemDrag;
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
            this.control.comboBox1.Items.Clear();
            this.control.comboBox2.Items.Clear();
            this.control.listView1.Items.Clear();

            if (this.Sections == null)
            {
                return;
            }

            this.PopulateWorldComboBox();
        }

        private void PopulateWorldComboBox()
        {
            var worlds = this.Sections
                .Select(x => x.World)
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

            var selectedWorld = (string)this.control.comboBox1.SelectedItem;

            var categories = this.Sections
                .Where(x => x.World == selectedWorld)
                .Select(x => x.Category)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

            foreach (var cat in categories)
            {
                this.control.comboBox2.Items.Add(cat);
            }

            if (this.control.comboBox2.Items.Count > 0)
            {
                this.control.comboBox2.SelectedIndex = 0;
            }
        }

        private void PopulateListView()
        {
            this.control.listView1.Items.Clear();

            var selectedWorld = (string)this.control.comboBox1.SelectedItem;
            var selectedCategory = (string)this.control.comboBox2.SelectedItem;

            var sections = this.Sections
                .Where(x => x.World == selectedWorld && x.Category == selectedCategory)
                .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            var images = new ImageList();
            images.ImageSize = new Size(128, 128);
            foreach (var s in sections)
            {
                images.Images.Add(s.Minimap);
            }

            this.control.listView1.LargeImageList = images;

            var i = 0;
            foreach (var s in sections)
            {
                var label = $"{s.Name} ({s.PixelWidth}x{s.PixelHeight})";
                var item = new ListViewItem(label, i++) { Tag = s };
                this.control.listView1.Items.Add(item);
            }
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopulateCategoryComboBox();
        }

        private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopulateListView();
        }

        private void ListViewItemDrag(object sender, ItemDragEventArgs e)
        {
            var view = (ListView)sender;

            if (view.SelectedIndices.Count == 0)
            {
                return;
            }

            var id = ((Section)view.SelectedItems[0].Tag).Id;
            view.DoDragDrop(id.ToString(), DragDropEffects.Copy);
        }
    }
}
