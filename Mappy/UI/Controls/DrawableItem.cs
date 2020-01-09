namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.UI.Drawables;
    using Mappy.Util;

    public class DrawableItem : Notifier, IQuadTreeItem
    {
        private bool visible = true;
        private bool locked;

        public DrawableItem(int x, int y, int z, IDrawable drawable)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Drawable = drawable;
            drawable.AreaChanged += this.DrawableOnAreaChanged;
        }

        public event EventHandler<AreaChangedEventArgs> AreaChanged;

        public int X { get; }

        public int Y { get; }

        public int Z { get; }

        public IDrawable Drawable { get; }

        public Rectangle Bounds => new Rectangle(
            this.X,
            this.Y,
            this.Drawable.Width,
            this.Drawable.Height);

        public bool Locked
        {
            get { return this.locked; }
            set { this.SetField(ref this.locked, value, "Locked"); }
        }

        public bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                if (this.SetField(ref this.visible, value, "Visible"))
                {
                    this.AreaChanged?.Invoke(this, new AreaChangedEventArgs(this.Bounds));
                }
            }
        }

        public object Tag { get; set; }

        public Point GetMidPoint()
        {
            return new Point(this.X + (this.Drawable.Width / 2), this.Y + (this.Drawable.Height / 2));
        }

        public Rectangle GetRect()
        {
            return new Rectangle(
                this.X,
                this.Y,
                this.Drawable.Width,
                this.Drawable.Height);
        }

        public void Draw(Graphics g, Rectangle clip)
        {
            g.TranslateTransform(this.X, this.Y);
            clip.Offset(-this.X, -this.Y);
            this.Drawable.Draw(g, clip);
            g.TranslateTransform(-this.X, -this.Y);
        }

        private void DrawableOnAreaChanged(object sender, AreaChangedEventArgs areaChangedEventArgs)
        {
            var rect = areaChangedEventArgs.ChangedRectangle;
            rect.Offset(this.X, this.Y);
            this.AreaChanged?.Invoke(this, new AreaChangedEventArgs(rect));
        }
    }
}