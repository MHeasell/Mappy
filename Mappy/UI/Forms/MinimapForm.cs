namespace Mappy.UI.Forms
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Models;

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
                case "ViewportLocation":
                case "ViewportWidth":
                case "ViewportHeight":
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

            int x = loc.X - (this.minimapControl.ViewportRect.Width / 2);
            int y = loc.Y - (this.minimapControl.ViewportRect.Height / 2);

            x = this.ScaleWidthToMap(x);
            y = this.ScaleHeightToMap(y);

            this.model.SetViewportLocation(new Point(x, y));
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

        private int ScaleWidthToMinimap(int val)
        {
            int mapWidth = (this.model.MapWidth * 32) - 32;
            int minimapWidth = this.minimapControl.BackgroundImage.Width;
            return (val * minimapWidth) / mapWidth;
        }

        private int ScaleWidthToMap(int val)
        {
            int mapWidth = (this.model.MapWidth * 32) - 32;
            int minimapWidth = this.minimapControl.BackgroundImage.Width;
            return (val * mapWidth) / minimapWidth;
        }

        private int ScaleHeightToMinimap(int val)
        {
            int mapHeight = (this.model.MapHeight * 32) - 128;
            int minimapHeight = this.minimapControl.BackgroundImage.Height;
            return (val * minimapHeight) / mapHeight;
        }

        private int ScaleHeightToMap(int val)
        {
            int mapHeight = (this.model.MapHeight * 32) - 128;
            int minimapHeight = this.minimapControl.BackgroundImage.Height;
            return (val * mapHeight) / minimapHeight;
        }

        private void UpdateViewportRectangle()
        {
            if (this.minimapControl.BackgroundImage == null)
            {
                this.minimapControl.ViewportRect = Rectangle.Empty;
                return;
            }

            var rectangle = new Rectangle(
                this.ScaleWidthToMinimap(this.model.ViewportLocation.X),
                this.ScaleHeightToMinimap(this.model.ViewportLocation.Y),
                this.ScaleWidthToMinimap(this.model.ViewportWidth),
                this.ScaleHeightToMinimap(this.model.ViewportHeight));

            this.minimapControl.ViewportRect = rectangle;
        }
    }
}
