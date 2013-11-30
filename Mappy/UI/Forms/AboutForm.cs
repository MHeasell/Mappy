namespace Mappy.UI.Forms
{
    using System.Windows.Forms;

    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            this.InitializeComponent();

            this.richTextBox1.Text = Mappy.Properties.Resources.AboutText;
            this.label1.Text = string.Format(
                "{0} v{1}",
                Application.ProductName,
                Application.ProductVersion);
        }
    }
}
