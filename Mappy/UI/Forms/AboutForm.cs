namespace Mappy.UI.Forms
{
    using System.Windows.Forms;

    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            this.InitializeComponent();

            this.richTextBox1.Text = Mappy.Properties.Resources.AboutText;
        }
    }
}
