namespace Mappy.UI.Controls
{
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
    }
}
