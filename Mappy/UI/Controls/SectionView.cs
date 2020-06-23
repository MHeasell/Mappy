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

        protected bool SuppressCombo1SelectedItemEvents { get; set; }

        protected bool SuppressCombo2SelectedItemEvents { get; set; }

        protected ListViewItem PreviousSelection { get; set; }

        public void SetModel(ISectionViewViewModel model)
        {
            model.ComboBox1Model.Buffer(2, 1).Subscribe(xs => this.UpdateComboBox1(xs[0], xs[1]));

            model.ComboBox2Model.Buffer(2, 1).Subscribe(xs => this.UpdateComboBox2(xs[0], xs[1]));

            model.ListViewItems.Subscribe(this.UpdateListView);

            this.model = model;
        }

        public ListViewItem GetCurrentSelectedItem()
        {
            return this.PreviousSelection;
        }

        protected static void UpdateComboBox(ComboBox c, ComboBoxViewModel oldModel, ComboBoxViewModel newModel)
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

        protected void UpdateComboBox1(ComboBoxViewModel oldModel, ComboBoxViewModel newModel)
        {
            this.SuppressCombo1SelectedItemEvents = true;
            UpdateComboBox(this.control.ComboBox1, oldModel, newModel);
            this.SuppressCombo1SelectedItemEvents = false;
        }

        protected void UpdateComboBox2(ComboBoxViewModel oldModel, ComboBoxViewModel newModel)
        {
            this.SuppressCombo2SelectedItemEvents = true;
            UpdateComboBox(this.control.ComboBox2, oldModel, newModel);
            this.SuppressCombo2SelectedItemEvents = false;
        }

        protected void UpdateListView(IEnumerable<Models.ListViewItem> xs)
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
            var i = 0;
            foreach (var x in sections)
            {
                var item = new ListViewItem(x.Name, i++) { Tag = x.Tag };
                lv.Items.Add(item);
            }

            lv.EndUpdate();
        }

        protected void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SuppressCombo1SelectedItemEvents)
            {
                return;
            }

            this.model.SelectComboBox1Item(this.control.ComboBox1.SelectedIndex);
        }

        protected void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SuppressCombo2SelectedItemEvents)
            {
                return;
            }

            this.model.SelectComboBox2Item(this.control.ComboBox2.SelectedIndex);
        }

        protected void ListViewItemDrag(object sender, ItemDragEventArgs e)
        {
            var view = (ListView)sender;

            if (view.SelectedIndices.Count == 0)
            {
                return;
            }

            var data = view.SelectedItems[0].Tag;
            view.DoDragDrop(data, DragDropEffects.Copy);
        }

        protected void ListViewItemSelectionChanged(object sender, EventArgs e)
        {
            var view = (ListView)sender;
            if (this.model.SectionType != SectionType.Feature || view.SelectedItems.Count == 0)
            {
                return;
            }

            view.ItemSelectionChanged -= this.ListViewItemSelectionChanged;
            var selItem = view.SelectedItems[0];

            if (this.PreviousSelection == null || this.PreviousSelection.Index == -1 || view.Items[this.PreviousSelection.Index] == null)
            {
                this.PreviousSelection = selItem;
            }

            if (selItem != this.PreviousSelection)
            {
                view.Items[this.PreviousSelection.Index].Selected = false;
                view.Items[this.PreviousSelection.Index].BackColor = Color.White;
                view.Items[selItem.Index].Selected = true;
                this.PreviousSelection = selItem;
            }

            view.ItemSelectionChanged += this.ListViewItemSelectionChanged;
        }

        // Show visual cue to user about which feature will be placed when RightClicking
        protected void ListViewLeave(object sender, EventArgs e)
        {
            // Cheeky null check
            if (this.model?.SectionType == SectionType.Feature && this.PreviousSelection != null)
            {
                this.PreviousSelection.BackColor = Color.Orange;
                this.PreviousSelection.ForeColor = Color.Black;
            }
        }
    }
}
