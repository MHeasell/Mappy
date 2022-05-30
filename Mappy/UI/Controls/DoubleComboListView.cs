namespace Mappy.UI.Controls
{
    using System.Drawing;
    using System.Windows.Forms;

    public partial class DoubleComboListView : UserControl
    {
        public DoubleComboListView()
        {
            this.InitializeComponent();
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
                e.Graphics.DrawRectangle(Pens.Red, e.Bounds);
            }
        }
    }
}
