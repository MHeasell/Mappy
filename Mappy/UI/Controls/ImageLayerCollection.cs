namespace Mappy.UI.Controls
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Linq;
    using Drawables;
    using Util;

    public class ImageLayerCollection : INotifyCollectionChanged
    {
        private QuadTree<Item> items;

        public ImageLayerCollection(int width, int height)
        {
            this.items = new QuadTree<Item>(new Rectangle(0, 0, width, height));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(Item item)
        {
            this.items.Add(item);
            this.OnAdd(item);
        }

        public bool Remove(Item item)
        {
            bool success = this.items.Remove(item);

            if (success)
            {
                this.OnRemove(item);
            }

            return success;
        }

        public void Clear()
        {
            this.items.Clear();
            this.OnClear();
        }

        public IEnumerable<Item> EnumerateIntersecting(Rectangle rect)
        {
            var l = this.items.FindInArea(rect).Where(x => x.Visible).ToList();
            l.Sort(new ZComparer());
            return l;
        }

        public Item HitTest(Point p)
        {
            var l = this.items.FindAtPoint(p).Where(x => !x.Locked && x.Visible).ToList();
            l.Sort(new ZComparer());
            return l.LastOrDefault();
        }

        public void Resize(int newWidth, int newHeight)
        {
            this.items = new QuadTree<Item>(new Rectangle(0, 0, newWidth, newHeight), this.items);
        }

        protected virtual void OnAdd(Item item)
        {
            NotifyCollectionChangedEventHandler h = this.CollectionChanged;
            if (h != null)
            {
                h(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        protected virtual void OnClear()
        {
            NotifyCollectionChangedEventHandler h = this.CollectionChanged;
            if (h != null)
            {
                h(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected virtual void OnRemove(Item item)
        {
            NotifyCollectionChangedEventHandler h = this.CollectionChanged;
            if (h != null)
            {
                h(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
        }

        public class Item : Notifier, QuadTree<Item>.IQuadTreeItem
        {
            private readonly int x;
            private readonly int y;
            private readonly int z;
            private readonly IDrawable drawable;

            private bool visible = true;
            private bool locked;

            public Item(int x, int y, int z, IDrawable drawable)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.drawable = drawable;
            }

            public int X
            {
                get { return this.x; }
            }

            public int Y
            {
                get { return this.y; }
            }

            public int Z
            {
                get { return this.z; }
            }

            public IDrawable Drawable
            {
                get { return this.drawable; }
            }

            public Rectangle Bounds
            {
                get { return new Rectangle(this.X, this.Y, this.Drawable.Width, this.Drawable.Height); }
            }

            public bool Locked
            {
                get { return this.locked; }
                set { this.SetField(ref this.locked, value, "Locked"); }
            }

            public bool Visible
            {
                get { return this.visible; }
                set { this.SetField(ref this.visible, value, "Visible"); }
            }

            public object Tag { get; set; }

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
        }

        private class ZComparer : IComparer<Item>
        {
            public int Compare(Item x, Item y)
            {
                return x.Z.CompareTo(y.Z);
            }
        }
    }
}