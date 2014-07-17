namespace Mappy.Minimap
{
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.UI.Forms;

    /// <summary>
    /// Presenter for the minimap viewport.
    /// Keeps the viewport in sync with the currently displayed map
    /// and the position of the main viewport.
    /// </summary>
    public class MinimapPresenter
    {
        private readonly IMinimapView minimap;

        private readonly IMinimapModel model;

        private readonly MainForm mainView;

        private bool mouseDown;

        public MinimapPresenter(IMinimapView mini, MainForm main, IMinimapModel model)
        {
            this.minimap = mini;
            this.mainView = main;
            this.model = model;

            this.model.PropertyChanged += this.ModelOnPropertyChanged;

            this.minimap.Visible = this.model.MinimapVisible;
            this.minimap.ViewportRectangle = this.model.ViewportRectangle;
            this.minimap.MinimapImage = this.model.MinimapImage;
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
                case "MinimapImage":
                    this.minimap.MinimapImage = this.model.MinimapImage;
                    break;
            }
        }
    }
}
