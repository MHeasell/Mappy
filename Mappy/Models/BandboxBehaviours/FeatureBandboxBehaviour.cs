namespace Mappy.Models.BandboxBehaviours
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Mappy.UI.Controls;
    using Mappy.UI.Tags;
    using Mappy.Util;

    public class FeatureBandboxBehaviour : Notifier, IBandboxBehaviour
    {
        private readonly ImageLayerView view;

        private readonly IFeatureBandboxModel model;

        private Point bandboxStartPoint;

        private Point bandboxFinishPoint;

        private Rectangle bandboxRectangle;

        public FeatureBandboxBehaviour(ImageLayerView view, IFeatureBandboxModel model)
        {
            this.view = view;
            this.model = model;
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
            var items = this.view.Items.EnumerateIntersecting(this.BandboxRectangle);
            var filteredItems = items.Where(x => x.Visible && !x.Locked && x.Tag is FeatureTag);

            var indices = filteredItems.Select(x => ((FeatureTag)x.Tag).FeatureId);

            this.model.SelectFeatures(indices);

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

    public interface IFeatureBandboxModel
    {
        void SelectFeatures(IEnumerable<Guid> indices);
    }
}