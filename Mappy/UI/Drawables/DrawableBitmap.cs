namespace Mappy.UI.Drawables
{
    using System.Drawing;

    public class DrawableBitmap : IDrawable
    {
        private readonly Bitmap bitmap;

        public DrawableBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public System.Drawing.Size Size
        {
            get { return this.bitmap.Size; }
        }

        public int Width
        {
            get { return this.bitmap.Width; }
        }

        public int Height
        {
            get { return this.bitmap.Height; }
        }

        public void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            graphics.DrawImageUnscaled(this.bitmap, 0, 0);
        }
    }
}
