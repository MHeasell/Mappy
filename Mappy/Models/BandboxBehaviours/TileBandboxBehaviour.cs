namespace Mappy.Models.BandboxBehaviours
{
    using System;
    using System.Drawing;

    using Mappy.Util;

    /// <summary>
    /// Defines a tile-based bandbox behaviour.
    /// In this mode, dragging out a bandbox selects an area of the map,
    /// in 32x32 tile increments.
    /// </summary>
    public class TileBandboxBehaviour : Notifier, IBandboxBehaviour
    {
        private readonly CoreModel model;

        private Rectangle bandboxRectangle;

        private Point startPoint;

        private Point finishPoint;

        private int bufferX;

        private int bufferY;

        public TileBandboxBehaviour(CoreModel model)
        {
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
            var p = new Point((x + 16) / 32, (y + 16) / 32);
            this.startPoint = p;
            this.finishPoint = p;
            this.UpdateBandboxRectangle();
            this.bufferX = x - (p.X * 32);
            this.bufferY = y - (p.Y * 32);
        }

        public void GrowBandbox(int x, int y)
        {
            this.bufferX += x;
            this.bufferY += y;

            while (this.bufferX >= 16)
            {
                this.finishPoint.X++;
                this.bufferX -= 32;
            }

            while (this.bufferY >= 16)
            {
                this.finishPoint.Y++;
                this.bufferY -= 32;
            }

            while (this.bufferX < -16)
            {
                this.finishPoint.X--;
                this.bufferX += 32;
            }

            while (this.bufferY < -16)
            {
                this.finishPoint.Y--;
                this.bufferY += 32;
            }

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

                int index = this.model.LiftArea(
                    this.BandboxRectangle.X / 32,
                    this.BandboxRectangle.Y / 32,
                    this.BandboxRectangle.Width / 32,
                    this.bandboxRectangle.Height / 32);

                this.model.SelectTile(index);
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
