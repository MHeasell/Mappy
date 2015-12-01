namespace Mappy.UI.Forms
{
    using System;
    using System.Drawing;
    using System.Reactive.Linq;
    using System.Windows.Forms;

    using Mappy.Models;

    public partial class MinimapForm : Form
    {
        private IMinimapModel model;

        private IUserEventDispatcher dispatcher;

        private bool mouseDown;

        private int mapWidth;

        private int mapHeight;

        public MinimapForm()
        {
            this.InitializeComponent();
        }

        public void SetModel(IMinimapModel model)
        {
            this.model = model;

            model.MapWidth.Subscribe(x => this.mapWidth = x);
            model.MapHeight.Subscribe(x => this.mapHeight = x);

            model.MinimapVisible.Subscribe(x => this.Visible = x);
            model.MinimapImage.Subscribe(x => this.minimapControl.BackgroundImage = x);

            // transform the coordinates for the minimap viewport rectangle
            var width = this.ScaleObsWidthToMinimap(model.ViewportWidth);
            var height = this.ScaleObsHeightToMinimap(model.ViewportHeight);
            var locX = this.ScaleObsWidthToMinimap(model.ViewportLocation.Select(x => x.X));
            var locY = this.ScaleObsHeightToMinimap(model.ViewportLocation.Select(x => x.Y));
            var loc = locX.CombineLatest(locY, (x, y) => new Point(x, y));
            var size = width.CombineLatest(height, (w, h) => new Size(w, h));
            var rect = loc.CombineLatest(size, (l, s) => new Rectangle(l, s));

            var empty = Observable.Return(Rectangle.Empty);
            model.MinimapImage
                .Select(x => x == null ? empty : rect)
                .Switch()
                .Subscribe(x => this.minimapControl.ViewportRect = x);
        }

        public void SetDispatcher(IUserEventDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        private IObservable<int> ScaleObsWidthToMinimap(IObservable<int> value)
        {
            var mapWidth = this.model.MapWidth.Select(x => (x * 32) - 32);
            var minimapWidth = this.model.MinimapImage.Select(x => x?.Width ?? 0);

            return value
                .CombineLatest(minimapWidth, (v, h) => v * h)
                .CombineLatest(mapWidth, (v, h) => v / h);
        }

        private IObservable<int> ScaleObsHeightToMinimap(IObservable<int> value)
        {
            var mapHeight = this.model.MapHeight.Select(x => (x * 32) - 128);
            var minimapHeight = this.model.MinimapImage.Select(x => x?.Height ?? 0);

            return value
                .CombineLatest(minimapHeight, (v, h) => v * h)
                .CombineLatest(mapHeight, (v, h) => v / h);
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
