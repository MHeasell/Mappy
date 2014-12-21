namespace Mappy.UI.Forms
{
    using System.Windows.Forms;

    public partial class ImportCustomSectionForm : Form
    {
        public ImportCustomSectionForm()
        {
            this.InitializeComponent();
        }

        public string GraphicPath
        {
            get
            {
                return this.textBox1.Text;
            }

            set
            {
                this.textBox1.Text = value;
            }
        }

        public string HeightmapPath
        {
            get
            {
                return this.textBox2.Text;
            }

            set
            {
                this.textBox2.Text = value;
            }
        }

        private void Button3Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Button4Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Button1Click(object sender, System.EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Open Graphic Image";
            dlg.Filter = "PNG images|*.png|All files|*.*";
            var result = dlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.GraphicPath = dlg.FileName;
            }
        }

        private void Button2Click(object sender, System.EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Open Heightmap Image";
            dlg.Filter = "Image files|*.png;*jpg;*.jpeg;*.gif;*.bmp|All files|*.*";
            var result = dlg.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.HeightmapPath = dlg.FileName;
            }
        }
    }
}
