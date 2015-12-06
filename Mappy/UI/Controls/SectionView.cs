namespace Mappy.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Forms;

    using Mappy.Models;

    using ListViewItem = System.Windows.Forms.ListViewItem;

    public partial class SectionView : UserControl
    {
        private ISectionViewViewModel model;

        private bool suppressCombo1SelectedItemEvents;
        private bool suppressCombo2SelectedItemEvents;

        public SectionView()
        {
            this.InitializeComponent();

            this.control.ComboBox1.SelectedIndexChanged += this.ComboBox1SelectedIndexChanged;
            this.control.ComboBox2.SelectedIndexChanged += this.ComboBox2SelectedIndexChanged;
            this.control.ListView.ItemDrag += this.ListViewItemDrag;
        }

        public void SetModel(ISectionViewViewModel model)
        {
            model.Worlds.Buffer(2, 1).Subscribe(xs => this.UpdateComboBox1(xs[0], xs[1]));

            model.Categories.Buffer(2, 1).Subscribe(xs => this.UpdateComboBox2(xs[0], xs[1]));

            model.Sections.Subscribe(this.UpdateListView);

            this.model = model;
        }

        private static void UpdateComboBox(ComboBox c, ComboBoxViewModel oldModel, ComboBoxViewModel newModel)
        {
            c.BeginUpdate();

            // yes we really want reference equality here
            if (oldModel.Items != newModel.Items)
            {
                c.Items.Clear();
                foreach (var x in newModel.Items)
                {
                    c.Items.Add(x);
                }
            }

            c.SelectedIndex = newModel.SelectedIndex;

            c.EndUpdate();
        }

        private void UpdateComboBox1(ComboBoxViewModel oldModel, ComboBoxViewModel newModel)
        {
            this.suppressCombo1SelectedItemEvents = true;
            UpdateComboBox(this.control.ComboBox1, oldModel, newModel);
            this.suppressCombo1SelectedItemEvents = false;
        }

        private void UpdateComboBox2(ComboBoxViewModel oldModel, ComboBoxViewModel newModel)
        {
            this.suppressCombo2SelectedItemEvents = true;
            UpdateComboBox(this.control.ComboBox2, oldModel, newModel);
            this.suppressCombo2SelectedItemEvents = false;
        }

        private void UpdateListView(IEnumerable<Models.ListViewItem> xs)
        {
            var sections = xs.ToList();

            var lv = this.control.ListView;

            lv.BeginUpdate();
            lv.Items.Clear();

            // update the images list
            var images = new ImageList { ImageSize = new Size(128, 128) };
            foreach (var x in sections)
            {
                images.Images.Add(x.Image);
            }

            lv.LargeImageList = images;

            // update the items list
            int i = 0;
            foreach (var x in sections)
            {
                var item = new ListViewItem(x.Name, i++) { Tag = x.Tag };
                lv.Items.Add(item);
            }

            lv.EndUpdate();
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.suppressCombo1SelectedItemEvents)
            {
                return;
            }

            this.model.SelectWorld(this.control.ComboBox1.SelectedIndex);
        }

        private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.suppressCombo2SelectedItemEvents)
            {
                return;
            }

            this.model.SelectCategory(this.control.ComboBox2.SelectedIndex);
        }

        private void ListViewItemDrag(object sender, ItemDragEventArgs e)
        {
            var view = (ListView)sender;

            if (view.SelectedIndices.Count == 0)
            {
                return;
            }

            var data = view.SelectedItems[0].Tag;
            view.DoDragDrop(data, DragDropEffects.Copy);
        }
    }
}
