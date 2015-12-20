namespace Mappy.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Linq;

    using Mappy.Collections;
    using Mappy.UI.Drawables;
    using Mappy.Util;

    public class DrawableItemCollection : ICollection<DrawableItemCollection.Item>, INotifyCollectionChanged
    {
        private QuadTree<Item> items;

        public DrawableItemCollection(int width, int height)
        {
            this.items = new QuadTree<Item>(new Rectangle(0, 0, width, height));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => this.items.Count;

        public bool IsReadOnly => false;

        public void Add(Item item)
        {
            this.items.Add(item);
            this.OnAdd(item);
        }

        public void CopyTo(Item[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
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

        public bool Contains(Item item)
        {
            return this.items.Contains(item);
        }

        public IEnumerable<Item> EnumerateIntersecting(Rectangle rect)
        {
            return this.items.FindInArea(rect).Where(x => x.Visible).OrderBy(x => x.Z);
        }

        public Item HitTest(Point p)
        {
            return this.items.FindAtPoint(p)
                .Where(x => !x.Locked && x.Visible)
                .OrderByDescending(x => x.Z)
                .FirstOrDefault();
        }

        public void Resize(int newWidth, int newHeight)
        {
            this.items = new QuadTree<Item>(new Rectangle(0, 0, newWidth, newHeight), this.items);
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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

        public class Item : Notifier, IQuadTreeItem
        {
            private bool visible = true;
            private bool locked;

            public Item(int x, int y, int z, IDrawable drawable)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.Drawable = drawable;
            }

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
    }
}