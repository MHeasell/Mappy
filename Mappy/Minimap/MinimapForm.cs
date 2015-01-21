namespace Mappy.Minimap
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class MinimapForm : Form
    {
        private IMinimapModel model;

        private bool mouseDown;

        public MinimapForm()
        {
            this.InitializeComponent();
        }

        public void SetModel(IMinimapModel model)
        {
            this.model = model;

            model.PropertyChanged += this.ModelOnPropertyChanged;

            this.Visible = model.MinimapVisible;
            this.minimapControl.BackgroundImage = model.MinimapImage;
            this.UpdateViewportRectangle();
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MinimapVisible":
                    this.Visible = this.model.MinimapVisible;
                    break;
                case "ViewportRectangle":
                    this.UpdateViewportRectangle();
                    break;
                case "MinimapImage":
                    this.minimapControl.BackgroundImage = this.model.MinimapImage;
                    this.UpdateViewportRectangle();
                    break;
            }
        }

        private void MinimapFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;

                this.model.HideMinimap();
            }
        }

        private void MinimapControl1MouseDown(object sender, MouseEventArgs e)
        {
            this.mouseDown = true;
            this.SetModelViewportCenter(e.Location);
        }

        private void SetModelViewportCenter(Point loc)
        {
            if (this.model.MinimapImage == null)
            {
                return;
            }

            double x = loc.X / (double)this.model.MinimapImage.Width;
            double y = loc.Y / (double)this.model.MinimapImage.Height;

            this.model.SetViewportCenterNormalized(x, y);
        }

        private void MinimapControl1MouseMove(object sender, MouseEventArgs e)
        {
            if (this.mouseDown)
            {
                this.SetModelViewportCenter(e.Location);
            }
        }

        private void MinimapControl1MouseUp(object sender, MouseEventArgs e)
        {
            this.mouseDown = false;
        }

        private void UpdateViewportRectangle()
        {
            if (this.minimapControl.BackgroundImage == null)
            {
                return;
            }

            var rectangle = this.model.ViewportRectangle;

            int w = this.model.MinimapImage.Width;
            int h = this.model.MinimapImage.Height;

            this.minimapControl.ViewportRect = new Rectangle(
                (int)(rectangle.MinX * w),
                (int)(rectangle.MinY * h),
                (int)(rectangle.Width * w),
                (int)(rectangle.Height * h));
        }
    }
}
