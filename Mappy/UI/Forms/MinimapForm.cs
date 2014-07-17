namespace Mappy.UI.Forms
{
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Controllers;

    public partial class MinimapForm : Form
    {
        private RectangleF viewportRectangle;

        public MinimapForm()
        {
            this.InitializeComponent();
        }

        public MinimapController Presenter { get; set; }

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

        public RectangleF ViewportRectangle
        {
            get
            {
                return this.viewportRectangle;
            }

            set
            {
                this.viewportRectangle = value;
                this.UpdateViewportRect();
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

        private void UpdateViewportRect()
        {
            this.minimapControl1.ViewportRect = this.ConvertToMinimapRect(this.ViewportRectangle);
        }

        private Rectangle ConvertToMinimapRect(RectangleF rectangle)
        {
            int w = this.minimapControl1.Width;
            int h = this.minimapControl1.Height;

            return new Rectangle(
                (int)(rectangle.X * w),
                (int)(rectangle.Y * h),
                (int)(rectangle.Width * w),
                (int)(rectangle.Height * h));
        }

        private void MinimapControl1MouseDown(object sender, MouseEventArgs e)
        {
            var pos = this.ToNormalizedPosition(e.Location);
            this.Presenter.MinimapClick(pos);
        }

        private void MinimapControl1MouseMove(object sender, MouseEventArgs e)
        {
            var pos = this.ToNormalizedPosition(e.Location);
            this.Presenter.MinimapMouseMove(pos);
        }

        private void MinimapControl1MouseUp(object sender, MouseEventArgs e)
        {
            var pos = this.ToNormalizedPosition(e.Location);
            this.Presenter.MinimapMouseUp(pos);
        }

        private PointF ToNormalizedPosition(Point location)
        {
            return new PointF(
                location.X / (float)this.minimapControl1.Width,
                location.Y / (float)this.minimapControl1.Height);
        }
    }
}
