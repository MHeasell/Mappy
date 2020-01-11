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
        private ListViewItem previousSelection;

        public SectionView()
        {
            this.InitializeComponent();

            this.control.ComboBox1.SelectedIndexChanged += this.ComboBox1SelectedIndexChanged;
            this.control.ComboBox2.SelectedIndexChanged += this.ComboBox2SelectedIndexChanged;
            this.control.ListView.ItemDrag += this.ListViewItemDrag;
            this.control.ListView.SelectedIndexChanged += this.ListViewItemSelectionChanged;
            this.control.ListView.Leave += this.ListViewLeave;
        }

        public Size ImageSize { get; set; } = new Size(128, 128);

        public void SetModel(ISectionViewViewModel model)
        {
            model.ComboBox1Model.Buffer(2, 1).Subscribe(xs => this.UpdateComboBox1(xs[0], xs[1]));

            model.ComboBox2Model.Buffer(2, 1).Subscribe(xs => this.UpdateComboBox2(xs[0], xs[1]));

            model.ListViewItems.Subscribe(this.UpdateListView);

            this.model = model;
        }

        public ListViewItem GetCurrentSelectedItem()
        {
            return this.previousSelection;
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
            var images = new ImageList { ImageSize = this.ImageSize };
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

            this.model.SelectComboBox1Item(this.control.ComboBox1.SelectedIndex);
        }

        private void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.suppressCombo2SelectedItemEvents)
            {
                return;
            }

            this.model.SelectComboBox2Item(this.control.ComboBox2.SelectedIndex);
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

        private void ListViewItemSelectionChanged(object sender, EventArgs e)
        {
            var view = (ListView)sender;
            if (this.model.SectionType != SectionType.Feature || view.SelectedItems.Count == 0)
            {
                return;
            }

            view.ItemSelectionChanged -= this.ListViewItemSelectionChanged;
            var selItem = view.SelectedItems[0];

            if (this.previousSelection == null || this.previousSelection.Index == -1 || view.Items[this.previousSelection.Index] == null)
            {
                this.previousSelection = selItem;
            }

            if (selItem != this.previousSelection)
            {
                view.Items[this.previousSelection.Index].Selected = false;
                view.Items[this.previousSelection.Index].BackColor = Color.White;
                view.Items[selItem.Index].Selected = true;
                this.previousSelection = selItem;
            }

            view.ItemSelectionChanged += this.ListViewItemSelectionChanged;
        }

        // Show visual cue to user about which feature will be placed when RightClicking
        private void ListViewLeave(object sender, EventArgs e)
        {
            // Cheeky null check
            if (this.model.SectionType == SectionType.Feature && this.previousSelection != null)
            {
                this.previousSelection.BackColor = Color.Orange;
                this.previousSelection.ForeColor = Color.Black;
            }
        }
    }
}
