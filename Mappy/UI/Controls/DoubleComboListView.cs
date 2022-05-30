namespace Mappy.UI.Controls
{
    using System.Drawing;
    using System.Windows.Forms;

    public partial class DoubleComboListView : UserControl
    {
        private readonly Pen selectionPen;
        private readonly Pen defaultPen;

        private ListViewItem previousSelection;

        public DoubleComboListView()
        {
            this.InitializeComponent();
            this.selectionPen = new Pen(Color.Red, 3);
            this.defaultPen = new Pen(Color.White, 3);
        }

        public ComboBox ComboBox1 => this.comboBox1;

        public ComboBox ComboBox2 => this.comboBox2;

        public ListView ListView => this.listView1;

        private void ListView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            if (e.Item.Selected)
            {
                e.Graphics.DrawRectangle(this.selectionPen, e.Bounds);
                this.previousSelection = e.Item;
            }
            else if (sender is ListView && ((ListView)sender).SelectedItems.Count <= 0 &&
                this.previousSelection != null && this.previousSelection.Index == e.Item.Index)
            {
                e.Graphics.DrawRectangle(this.selectionPen, this.previousSelection.Bounds);
            }
            else
            {
                e.Graphics.DrawRectangle(this.defaultPen, e.Bounds);
            }
        }
    }
}
