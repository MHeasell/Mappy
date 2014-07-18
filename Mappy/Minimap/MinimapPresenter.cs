namespace Mappy.Minimap
{
    using System.ComponentModel;
    using System.Drawing;

    /// <summary>
    /// Presenter for the minimap viewport.
    /// Keeps the viewport in sync with the currently displayed map
    /// and the position of the main viewport.
    /// </summary>
    public class MinimapPresenter
    {
        private readonly IMinimapView minimap;

        private readonly IMinimapModel model;

        private readonly IMinimapService service;

        private bool mouseDown;

        public MinimapPresenter(IMinimapView mini, IMinimapService service, IMinimapModel model)
        {
            this.minimap = mini;
            this.service = service;
            this.model = model;

            this.model.PropertyChanged += this.ModelOnPropertyChanged;

            this.minimap.Visible = this.model.MinimapVisible;
            this.minimap.MinimapImage = this.model.MinimapImage;
            this.UpdateViewportRectangle();
        }

        public void MinimapClick(Point location)
        {
            this.mouseDown = true;
            var normalizedLocation = this.ToNormalizedPosition(location);
            this.service.SetViewportCenterNormalized(normalizedLocation);
        }

        public void MinimapMouseMove(Point location)
        {
            if (this.mouseDown)
            {
                var normalizedLocation = this.ToNormalizedPosition(location);
                this.service.SetViewportCenterNormalized(normalizedLocation);
            }
        }

        public void MinimapMouseUp(Point location)
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
                    this.UpdateViewportRectangle();
                    break;
                case "MinimapImage":
                    this.minimap.MinimapImage = this.model.MinimapImage;
                    this.UpdateViewportRectangle();
                    break;
            }
        }

        private void UpdateViewportRectangle()
        {
            this.minimap.ViewportRectangle = this.ConvertToMinimapRect(this.model.ViewportRectangle);
        }

        private Rectangle ConvertToMinimapRect(RectangleF rectangle)
        {
            if (this.model.MinimapImage == null)
            {
                return Rectangle.Empty;
            }

            int w = this.model.MinimapImage.Width;
            int h = this.model.MinimapImage.Height;

            return new Rectangle(
                (int)(rectangle.X * w),
                (int)(rectangle.Y * h),
                (int)(rectangle.Width * w),
                (int)(rectangle.Height * h));
        }

        private PointF ToNormalizedPosition(Point location)
        {
            if (this.minimap.MinimapImage == null)
            {
                return PointF.Empty;
            }

            return new PointF(
                location.X / (float)this.minimap.MinimapImage.Width,
                location.Y / (float)this.minimap.MinimapImage.Height);
        }
    }
}
