namespace Mappy.UI.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Models;

    public partial class MinimapForm : Form
    {
        private IUserEventDispatcher dispatcher;

        private bool mouseDown;

        private int mapWidth;

        private int mapHeight;

        public MinimapForm()
        {
            this.InitializeComponent();
        }

        public void SetModel(IMinimapFormViewModel model)
        {
            model.MapWidth.Subscribe(x => this.mapWidth = x);
            model.MapHeight.Subscribe(x => this.mapHeight = x);

            model.MinimapVisible.Subscribe(x => this.Visible = x);
            model.MinimapImage.Subscribe(x => this.minimapControl.BackgroundImage = x);
            model.MinimapRect.Subscribe(x => this.minimapControl.ViewportRect = x);
        }

        public void SetDispatcher(IUserEventDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        private void MinimapFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;

                this.dispatcher.HideMinimap();
            }
        }

        private void MinimapControl1MouseDown(object sender, MouseEventArgs e)
        {
            this.mouseDown = true;
            this.SetModelViewportCenter(e.Location);
        }

        private void SetModelViewportCenter(Point loc)
        {
            if (this.minimapControl.BackgroundImage == null)
            {
                return;
            }

            int x = loc.X - (this.minimapControl.ViewportRect.Width / 2);
            int y = loc.Y - (this.minimapControl.ViewportRect.Height / 2);

            x = this.ScaleWidthToMap(x);
            y = this.ScaleHeightToMap(y);

            this.dispatcher.SetViewportLocation(new Point(x, y));
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

        private int ScaleWidthToMap(int val)
        {
            int mapWidth = (this.mapWidth * 32) - 32;
            int minimapWidth = this.minimapControl.BackgroundImage.Width;
            return (val * mapWidth) / minimapWidth;
        }

        private int ScaleHeightToMap(int val)
        {
            int mapHeight = (this.mapHeight * 32) - 128;
            int minimapHeight = this.minimapControl.BackgroundImage.Height;
            return (val * mapHeight) / minimapHeight;
        }
    }
}
