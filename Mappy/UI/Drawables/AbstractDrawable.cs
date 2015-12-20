namespace Mappy.UI.Drawables
{
    using System;
    using System.Drawing;

    using Mappy.UI.Controls;

    public abstract class AbstractDrawable : IDrawable
    {
        public event EventHandler<AreaChangedEventArgs> AreaChanged;

        public abstract Size Size { get; }

        public abstract int Width { get; }

        public abstract int Height { get; }

        public abstract void Draw(Graphics graphics, Rectangle clipRectangle);

        protected void OnAreaChanged()
        {
            var rect = new Rectangle(0, 0, this.Width, this.Height);
            this.OnAreaChanged(new AreaChangedEventArgs(rect));
        }

        protected void OnAreaChanged(Rectangle rect)
        {
            this.OnAreaChanged(new AreaChangedEventArgs(rect));
        }

        protected virtual void OnAreaChanged(AreaChangedEventArgs e)
        {
            this.AreaChanged?.Invoke(this, e);
        }
    }
}
