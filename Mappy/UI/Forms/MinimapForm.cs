namespace Mappy.UI.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Mappy.Models;
    using Mappy.Views;

    public partial class MinimapForm : Form
    {
        private MainForm mainView;
        private IBindingMapModel map;

        private bool mouseDown;

        public MinimapForm()
        {
            this.InitializeComponent();
        }

        public event EventHandler<MinimapMoveEventArgs> ViewportMove;

        public MainForm MainView
        {
            get
            {
                return this.mainView;
            }

            set
            {
                if (this.mainView != null)
                {
                    this.mainView.ViewportLocationChanged -= this.ViewportLocationChanged;
                }

                this.mainView = value;

                if (this.mainView != null)
                {
                    this.mainView.ViewportLocationChanged += this.ViewportLocationChanged;
                }

                this.UpdateViewportRect();
            }
        }

        public IBindingMapModel Map
        {
            get
            {
                return this.map;
            }

            set
            {
                if (this.map != null)
                {
                    this.map.MinimapChanged -= this.MapMinimapChanged;
                }

                this.map = value;

                if (this.map != null)
                {
                    this.map.MinimapChanged += this.MapMinimapChanged;
                }

                this.UpdateMinimapPicture();
            }
        }

        private void UpdateMinimapPicture()
        {
            if (this.Map == null)
            {
                this.minimapControl1.BackgroundImage = null;
            }
            else
            {
                this.minimapControl1.BackgroundImage = this.Map.Minimap;
            }
        }

        private void OnViewportMove(Point location)
        {
            EventHandler<MinimapMoveEventArgs> h = this.ViewportMove;
            if (h != null)
            {
                h(this, new MinimapMoveEventArgs { Location = location });
            }
        }

        private void MinimapForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void ViewportLocationChanged(object sender, EventArgs e)
        {
            this.UpdateViewportRect();
        }

        private void UpdateViewportRect()
        {
            bool visible = this.mainView != null && this.mainView.Map != null;
            this.minimapControl1.RectVisible = visible;
            if (visible)
            {
                this.minimapControl1.ViewportRect = this.ConvertToMinimapRect(this.mainView.ViewportRect);
            }
        }

        private Rectangle ConvertToMinimapRect(Rectangle rectangle)
        {
            float facX = this.minimapControl1.Width / (this.mainView.Map.Tile.TileGrid.Width * 32.0f);
            float facY = this.minimapControl1.Height / (this.mainView.Map.Tile.TileGrid.Height * 32.0f);

            return new Rectangle(
                (int)(rectangle.X * facX),
                (int)(rectangle.Y * facY),
                (int)(rectangle.Width * facX),
                (int)(rectangle.Height * facY));
        }

        private void MinimapControl1MouseDown(object sender, MouseEventArgs e)
        {
            this.mouseDown = true;
            this.OnViewportMove(e.Location);
        }

        private void MinimapControl1MouseMove(object sender, MouseEventArgs e)
        {
            if (this.mouseDown)
            {
                this.OnViewportMove(e.Location);
            }
        }

        private void MinimapControl1MouseUp(object sender, MouseEventArgs e)
        {
            this.mouseDown = false;
        }

        private void MapMinimapChanged(object sender, EventArgs e)
        {
            this.UpdateMinimapPicture();
        }
    }
}
