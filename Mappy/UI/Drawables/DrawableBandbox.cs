namespace Mappy.UI.Drawables
{
    using System;
    using System.Drawing;

    public sealed class DrawableBandbox : AbstractDrawable, IDisposable
    {
        private readonly Brush fillBrush;

        private readonly Pen borderPen;

        public DrawableBandbox(Brush fillBrush, Pen borderPen, Size size)
        {
            this.fillBrush = fillBrush;
            this.borderPen = borderPen;
            this.Size = size;
        }

        public override Size Size { get; }

        public override int Width => this.Size.Width;

        public override int Height => this.Size.Height;

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

        public override void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            graphics.FillRectangle(this.fillBrush, 0, 0, this.Width - 1, this.Height - 1);
            graphics.DrawRectangle(this.borderPen, 0, 0, this.Width - 1, this.Height - 1);
        }

        public void Dispose()
        {
            this.fillBrush.Dispose();
            this.borderPen.Dispose();
        }
    }
}
