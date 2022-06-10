namespace Mappy.Models.BandboxBehaviours
{
    using System;
    using System.Drawing;

    using Mappy.Util;

    /// <summary>
    /// Defines a free-form bandbox behaviour.
    /// In this mode, dragging out a bandbox selects features within an
    /// area of the map
    /// </summary>
    public class FreeBandboxBehaviour : Notifier, IBandboxBehaviour
    {
        private readonly IBandboxModel model;

        private Rectangle bandboxRectangle;

        private Point startPoint;

        private Point finishPoint;

        public FreeBandboxBehaviour(IBandboxModel model)
        {
            this.model = model;
        }

        public Rectangle BandboxRectangle
        {
            get => this.bandboxRectangle;
            private set => this.SetField(ref this.bandboxRectangle, value, nameof(this.BandboxRectangle));
        }

        public void StartBandbox(int x, int y)
        {
            var p = new Point(x, y);
            this.startPoint = p;
            this.finishPoint = p;
            this.UpdateBandboxRectangle();
        }

        public void GrowBandbox(int x, int y)
        {
            this.finishPoint.X += x;
            this.finishPoint.Y += y;

            this.UpdateBandboxRectangle();
        }

        public void CommitBandbox()
        {
            try
            {
                var width = this.BandboxRectangle.Width;
                var height = this.BandboxRectangle.Height;

                if (width == 0 || height == 0)
                {
                    return;
                }
                /*
                this.model.LiftAndSelectArea(
                    this.BandboxRectangle.X,
                    this.BandboxRectangle.Y,
                    this.BandboxRectangle.Width,
                    this.bandboxRectangle.Height);*/
            }
            finally
            {
                this.BandboxRectangle = Rectangle.Empty;
            }
        }

        private void UpdateBandboxRectangle()
        {
            var minX = Math.Min(this.startPoint.X, this.finishPoint.X);
            var minY = Math.Min(this.startPoint.Y, this.finishPoint.Y);

            var maxX = Math.Max(this.startPoint.X, this.finishPoint.X);
            var maxY = Math.Max(this.startPoint.Y, this.finishPoint.Y);

            var width = maxX - minX;
            var height = maxY - minY;

            this.BandboxRectangle = new Rectangle(minX, minY, width, height);
        }
    }
}
