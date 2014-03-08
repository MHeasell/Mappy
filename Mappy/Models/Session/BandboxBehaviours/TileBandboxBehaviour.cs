namespace Mappy.Models.Session.BandboxBehaviours
{
    using System;
    using System.Drawing;

    using Mappy.Util;

    public class TileBandboxBehaviour : Notifier, IBandboxBehaviour
    {
        private readonly ISelectionCommandHandler selectionHandler;

        private readonly IMapCommandHandler handler;

        private Rectangle bandboxRectangle;

        private Point startPoint;

        private Point finishPoint;

        private int bufferX;

        private int bufferY;

        public TileBandboxBehaviour(IMapCommandHandler handler, ISelectionCommandHandler selectionHandler)
        {
            this.handler = handler;
            this.selectionHandler = selectionHandler;
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
            var p = new Point(x / 32, y / 32);
            this.startPoint = p;
            this.finishPoint = p;
            this.UpdateBandboxRectangle();
            this.bufferX = x % 32;
            this.bufferY = y % 32;
        }

        public void GrowBandbox(int x, int y)
        {
            this.bufferX += x;
            this.bufferY += y;

            this.finishPoint.X += this.bufferX / 32;
            this.finishPoint.Y += this.bufferY / 32;

            this.bufferX %= 32;
            this.bufferY %= 32;

            this.UpdateBandboxRectangle();
        }

        public void CommitBandbox()
        {
            try
            {
                int width = this.BandboxRectangle.Width / 32;
                int height = this.BandboxRectangle.Height / 32;

                if (width == 0 || height == 0)
                {
                    return;
                }

                int index = this.handler.LiftArea(
                    this.BandboxRectangle.X / 32,
                    this.BandboxRectangle.Y / 32,
                    this.BandboxRectangle.Width / 32,
                    this.bandboxRectangle.Height / 32);

                this.selectionHandler.SelectTile(index);
            }
            finally
            {
                this.BandboxRectangle = Rectangle.Empty;
            }
        }

        private void UpdateBandboxRectangle()
        {
            int minX = Math.Min(this.startPoint.X, this.finishPoint.X);
            int minY = Math.Min(this.startPoint.Y, this.finishPoint.Y);

            int maxX = Math.Max(this.startPoint.X, this.finishPoint.X);
            int maxY = Math.Max(this.startPoint.Y, this.finishPoint.Y);

            int width = maxX - minX;
            int height = maxY - minY;

            this.BandboxRectangle = new Rectangle(minX * 32, minY * 32, width * 32, height * 32);
        }
    }
}
