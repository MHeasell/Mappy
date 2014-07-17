namespace Mappy.Controllers
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Models;
    using Mappy.UI.Forms;

    using Views;

    /// <summary>
    /// Presenter for the minimap viewport.
    /// Keeps the viewport in sync with the currently displayed map
    /// and the position of the main viewport.
    /// </summary>
    public class MinimapController
    {
        private readonly MinimapForm minimap;

        private readonly CoreModel model;

        private readonly MainForm mainView;

        private bool mouseDown;

        public MinimapController(MinimapForm mini, MainForm main, CoreModel model)
        {
            this.minimap = mini;
            this.mainView = main;
            this.model = model;

            this.minimap.Visible = this.model.MinimapVisible;
            this.minimap.ViewportRectangle = this.model.ViewportRectangle;
            this.MapChanged();

            this.model.PropertyChanged += this.ModelOnPropertyChanged;
        }

        public void MinimapClick(PointF location)
        {
            this.mouseDown = true;
            this.mainView.SetViewportCenterNormalized(location);
        }

        public void MinimapMouseMove(PointF location)
        {
            if (this.mouseDown)
            {
                this.mainView.SetViewportCenterNormalized(location);
            }
        }

        public void MinimapMouseUp(PointF location)
        {
            this.mouseDown = false;
        }

        public void MinimapClose()
        {
            this.model.MinimapVisible = false;
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "MinimapVisible":
                    this.minimap.Visible = this.model.MinimapVisible;
                    break;
                case "ViewportRectangle":
                    this.minimap.ViewportRectangle = this.model.ViewportRectangle;
                    break;
                case "Map":
                    this.MapChanged();
                    break;
            }
        }

        private void MapChanged()
        {
            if (this.model.Map != null)
            {
                this.model.Map.MinimapChanged += this.MapOnMinimapChanged;
                this.minimap.MinimapImage = this.model.Map.Minimap;
            }
        }

        private void MapOnMinimapChanged(object sender, EventArgs eventArgs)
        {
            this.minimap.MinimapImage = this.model.Map.Minimap;
        }
    }
}
