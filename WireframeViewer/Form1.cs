namespace WireframeViewer
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Mappy.IO;
    using Mappy.Util;

    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "3do Files|*.3do|All Files|*.*";
            var result = dlg.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var path = dlg.FileName;
                var loader = new ModelEdgeReader();

                using (var s = File.OpenRead(path))
                {
                    loader.Read(s);
                }

                var bmp = Util.RenderWireframe(loader.Edges);

                this.pictureBox1.Image = bmp.Image;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
