namespace Mappy.Minimap
{
    using System.Drawing;
    using System.Windows.Forms;

    public partial class MinimapForm : Form, IMinimapView
    {
        public MinimapForm()
        {
            this.InitializeComponent();
        }

        public MinimapPresenter Presenter { get; set; }

        public Image MinimapImage
        {
            get
            {
                return this.minimapControl1.BackgroundImage;
            }

            set
            {
                this.minimapControl1.BackgroundImage = value;
            }
        }

        public Rectangle ViewportRectangle
        {
            get
            {
                return this.minimapControl1.ViewportRect;
            }

            set
            {
                this.minimapControl1.ViewportRect = value;
            }
        }

        public bool ViewportVisible
        {
            get
            {
                return this.minimapControl1.RectVisible;
            }

            set
            {
                this.minimapControl1.RectVisible = value;
            }
        }

        private void MinimapFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Presenter.MinimapClose();
            }
        }

        private void MinimapControl1MouseDown(object sender, MouseEventArgs e)
        {
            this.Presenter.MinimapClick(e.Location);
        }

        private void MinimapControl1MouseMove(object sender, MouseEventArgs e)
        {
            this.Presenter.MinimapMouseMove(e.Location);
        }

        private void MinimapControl1MouseUp(object sender, MouseEventArgs e)
        {
            this.Presenter.MinimapMouseUp(e.Location);
        }
    }
}
