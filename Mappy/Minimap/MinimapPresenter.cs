namespace Mappy.Minimap
{
    using System.ComponentModel;
    using System.Drawing;

    using Geometry;

    using Mappy.Data;

    /// <summary>
    /// Presenter for the minimap viewport.
    /// Keeps the viewport in sync with the currently displayed map
    /// and the position of the main viewport.
    /// </summary>
    public class MinimapPresenter
    {
        private readonly IMinimapView minimap;

        private readonly IMinimapModel model;

        private bool mouseDown;

        public MinimapPresenter(IMinimapView mini, IMinimapModel model)
        {
            this.minimap = mini;
            this.model = model;

            this.model.PropertyChanged += this.ModelOnPropertyChanged;

            this.minimap.Visible = this.model.MinimapVisible;
            this.minimap.MinimapImage = this.model.MinimapImage;
            this.UpdateViewportRectangle();
        }

        public void MinimapClick(Point location)
        {
            this.mouseDown = true;
            this.SetModelViewportCenter(location);
        }

        public void MinimapMouseMove(Point location)
        {
            if (this.mouseDown)
            {
                this.SetModelViewportCenter(location);
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

        private Rectangle ConvertToMinimapRect(Rectangle2D rectangle)
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
    }
}
