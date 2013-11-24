namespace Mappy.UI.Forms
{
    using System.Windows.Forms;

    public partial class MapSelectionForm : Form
    {
        public MapSelectionForm()
        {
            this.InitializeComponent();
        }

        public ListBox.ObjectCollection Items
        {
            get
            {
                return this.listBox1.Items;
            }
        }

        public object SelectedItem
        {
            get
            {
                return this.listBox1.SelectedItem;
            }
        }
    }
}
