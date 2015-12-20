namespace Mappy.UI.Controls
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;

    public class SelectableItemsLayer : AbstractLayer
    {
        private readonly HashSet<DrawableItemCollection.Item> selectedItems = new HashSet<DrawableItemCollection.Item>();

        public SelectableItemsLayer(int width, int height)
        {
            this.Items = new DrawableItemCollection(width, height);
            this.Items.CollectionChanged += this.ItemsChanged;
        }

        public DrawableItemCollection Items { get; }

        public DrawableItemCollection.Item HitTest(int x, int y)
        {
            return this.Items.HitTest(new Point(x, y));
        }

        public void AddToSelection(DrawableItemCollection.Item item)
        {
            this.InvalidateSelectionRect(item);

            this.selectedItems.Add(item);
        }

        public void RemoveFromSelection(DrawableItemCollection.Item item)
        {
            if (this.selectedItems.Remove(item))
            {
                this.InvalidateSelectionRect(item);
            }
        }

        public bool SelectedItemsContains(DrawableItemCollection.Item item)
        {
            return this.selectedItems.Contains(item);
        }

        public void ClearSelection()
        {
            foreach (var item in this.selectedItems)
            {
                this.InvalidateSelectionRect(item);
            }

            this.selectedItems.Clear();
        }

        public bool IsInSelection(int x, int y)
        {
            var item = this.HitTest(x, y);

            if (item == null)
            {
                return false;
            }

            return this.SelectedItemsContains(item);
        }

        protected override void DoDraw(Graphics graphics, Rectangle clipRectangle)
        {
            // draw items
            foreach (var i in this.Items.EnumerateIntersecting(clipRectangle))
            {
                i.Draw(graphics, clipRectangle);
                if (this.selectedItems.Contains(i))
                {
                    graphics.DrawRectangle(
                        Pens.Red,
                        i.X,
                        i.Y,
                        i.Drawable.Width,
                        i.Drawable.Height);
                }
            }
        }

        private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var i in e.NewItems)
                    {
                        this.OnAddItem((DrawableItemCollection.Item)i);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var i in e.OldItems)
                    {
                        this.OnRemoveItem((DrawableItemCollection.Item)i);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var i in e.OldItems)
                    {
                        this.OnRemoveItem((DrawableItemCollection.Item)i);
                    }

                    foreach (var i in e.NewItems)
                    {
                        this.OnAddItem((DrawableItemCollection.Item)i);
                    }

                    break;
            }
        }

        private void OnAddItem(DrawableItemCollection.Item item)
        {
            item.AreaChanged += this.ItemAreaChanged;
            this.OnLayerChanged(item.Bounds);
        }

        private void OnRemoveItem(DrawableItemCollection.Item item)
        {
            item.AreaChanged -= this.ItemAreaChanged;
            this.OnLayerChanged(item.Bounds);
        }

        private void ItemAreaChanged(object sender, AreaChangedEventArgs e)
        {
            this.OnLayerChanged(e.ChangedRectangle);
        }

        private void InvalidateSelectionRect(DrawableItemCollection.Item item)
        {
            var r = item.Bounds;
            this.OnLayerChanged(new Rectangle(r.Left, r.Top, r.Width, 1));
            this.OnLayerChanged(new Rectangle(r.Left, r.Top, 1, r.Height));
            this.OnLayerChanged(new Rectangle(r.Right, r.Top, 1, r.Height));
            this.OnLayerChanged(new Rectangle(r.Left, r.Bottom, r.Width + 1, 1));
        }
    }
}
