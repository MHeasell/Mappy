namespace Mappy.UI.Drawables
{
    using System.Drawing;

    public class DrawableBitmap : AbstractDrawable
    {
        private readonly Bitmap bitmap;

        public DrawableBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public override Size Size => this.bitmap.Size;

        public override int Width => this.bitmap.Width;

        public override int Height => this.bitmap.Height;

        public override void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            graphics.DrawImageUnscaled(this.bitmap, 0, 0);
        }
    }
}
