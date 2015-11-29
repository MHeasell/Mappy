namespace Mappy.UI.Drawables
{
    using System;
    using System.Drawing;

    public class DrawableBandbox : IDrawable, IDisposable
    {
        private readonly Brush fillBrush;

        private readonly Pen borderPen;

        public DrawableBandbox(Brush fillBrush, Pen borderPen, Size size)
        {
            this.fillBrush = fillBrush;
            this.borderPen = borderPen;
            this.Size = size;
        }

        public Size Size { get; private set; }

        public int Width
        {
            get
            {
                return this.Size.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.Size.Height;
            }
        }

        public static DrawableBandbox CreateSimple(Size size, Color color, Color borderColor)
        {
            return CreateSimple(size, color, borderColor, 1);
        }

        public static DrawableBandbox CreateSimple(
            Size size,
            Color color,
            Color borderColor,
            int borderWidth)
        {
            return new DrawableBandbox(
                new SolidBrush(color),
                new Pen(borderColor, borderWidth),
                size);
        }

        public void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            graphics.FillRectangle(this.fillBrush, 0, 0, this.Width - 1, this.Height - 1);
            graphics.DrawRectangle(this.borderPen, 0, 0, this.Width - 1, this.Height - 1);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.fillBrush.Dispose();
                this.borderPen.Dispose();
            }
        }
    }
}
