namespace Mappy.Models.Session.BandboxBehaviours
{
    using System;
    using System.Drawing;
    using System.Linq;

    using Mappy.Controllers.Tags;
    using Mappy.UI.Controls;
    using Mappy.Util;

    public class FeatureBandboxBehaviour : Notifier, IBandboxBehaviour
    {
        private readonly ImageLayerView view;

        private readonly ISelectionModel selectionModel;

        private Point bandboxStartPoint;

        private Point bandboxFinishPoint;

        private Rectangle bandboxRectangle;

        public FeatureBandboxBehaviour(ImageLayerView view, SelectionModel selectionModel)
        {
            this.view = view;
            this.selectionModel = selectionModel;
        }

        public Rectangle BandboxRectangle
        {
            get
            {
                return this.bandboxRectangle;
            }

            private set
            {
                this.SetField(ref this.bandboxRectangle, value, "BandboxRectangle");
            }
        }

        public void StartBandbox(int x, int y)
        {
            var p = new Point(x, y);
            this.bandboxStartPoint = p;
            this.bandboxFinishPoint = p;
            this.UpdateBandboxRectangle();
        }

        public void GrowBandbox(int x, int y)
        {
            this.bandboxFinishPoint.X += x;
            this.bandboxFinishPoint.Y += y;
            this.UpdateBandboxRectangle();
        }

        public void CommitBandbox()
        {
            var items = this.view.Items.EnumerateIntersecting(this.selectionModel.BandboxRectangle);
            var filteredItems = items.Where(x => x.Visible && !x.Locked && x.Tag is FeatureTag);

            foreach (var i in filteredItems)
            {
                var tag = (FeatureTag)i.Tag;
                this.selectionModel.SelectedFeatures.Add(tag.Index);
            }

            this.BandboxRectangle = Rectangle.Empty;
        }

        private void UpdateBandboxRectangle()
        {
            int minX = Math.Min(this.bandboxStartPoint.X, this.bandboxFinishPoint.X);
            int minY = Math.Min(this.bandboxStartPoint.Y, this.bandboxFinishPoint.Y);

            int maxX = Math.Max(this.bandboxStartPoint.X, this.bandboxFinishPoint.X);
            int maxY = Math.Max(this.bandboxStartPoint.Y, this.bandboxFinishPoint.Y);

            int width = maxX - minX;
            int height = maxY - minY;

            this.BandboxRectangle = new Rectangle(minX, minY, width, height);
        }
    }
}