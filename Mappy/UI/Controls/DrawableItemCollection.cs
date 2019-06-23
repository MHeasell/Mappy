namespace Mappy.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Linq;

    using Mappy.Collections;

    public class DrawableItemCollection : ICollection<DrawableItem>, INotifyCollectionChanged
    {
        private QuadTree<DrawableItem> items;

        public DrawableItemCollection(int width, int height)
        {
            this.items = new QuadTree<DrawableItem>(new Rectangle(0, 0, width, height));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => this.items.Count;

        public bool IsReadOnly => false;

        public void Add(DrawableItem item)
        {
            this.items.Add(item);
            this.OnAdd(item);
        }

        public void CopyTo(DrawableItem[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public bool Remove(DrawableItem item)
        {
            var success = this.items.Remove(item);

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

        public bool Contains(DrawableItem item)
        {
            return this.items.Contains(item);
        }

        public IEnumerable<DrawableItem> EnumerateIntersecting(Rectangle rect)
        {
            return this.items.FindInArea(rect).Where(x => x.Visible).OrderBy(x => x.Z);
        }

        public DrawableItem HitTest(Point p)
        {
            return this.items.FindAtPoint(p)
                .Where(x => !x.Locked && x.Visible)
                .OrderByDescending(x => x.Z)
                .FirstOrDefault();
        }

        public void Resize(int newWidth, int newHeight)
        {
            this.items = new QuadTree<DrawableItem>(new Rectangle(0, 0, newWidth, newHeight), this.items);
        }

        public IEnumerator<DrawableItem> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected virtual void OnAdd(DrawableItem item)
        {
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            this.CollectionChanged?.Invoke(this, e);
        }

        protected virtual void OnClear()
        {
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            this.CollectionChanged?.Invoke(this, e);
        }

        protected virtual void OnRemove(DrawableItem item)
        {
            var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
            this.CollectionChanged?.Invoke(this, e);
        }
    }
}