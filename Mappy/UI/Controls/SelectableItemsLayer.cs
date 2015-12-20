namespace Mappy.UI.Controls
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;

    public class SelectableItemsLayer : AbstractLayer
    {
        private readonly HashSet<ImageLayerCollection.Item> selectedItems = new HashSet<ImageLayerCollection.Item>();

        public SelectableItemsLayer(int width, int height)
        {
            this.Items = new ImageLayerCollection(width, height);
            this.Items.CollectionChanged += this.ItemsChanged;
        }

        public ImageLayerCollection Items { get; }

        public ImageLayerCollection.Item HitTest(int x, int y)
        {
            return this.Items.HitTest(new Point(x, y));
        }

        public void AddToSelection(ImageLayerCollection.Item item)
        {
            this.InvalidateSelectionRect(item);

            this.selectedItems.Add(item);
        }

        public void RemoveFromSelection(ImageLayerCollection.Item item)
        {
            if (this.selectedItems.Remove(item))
            {
                this.InvalidateSelectionRect(item);
            }
        }

        public bool SelectedItemsContains(ImageLayerCollection.Item item)
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
                        ImageLayerCollection.Item item = (ImageLayerCollection.Item)i;
                        item.PropertyChanged += this.ItemPropertyChanged;
                        this.OnLayerChanged(item.Bounds);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var i in e.OldItems)
                    {
                        ImageLayerCollection.Item item = (ImageLayerCollection.Item)i;
                        item.PropertyChanged -= this.ItemPropertyChanged;
                        this.OnLayerChanged(item.Bounds);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var i in e.OldItems)
                    {
                        ImageLayerCollection.Item item = (ImageLayerCollection.Item)i;
                        item.PropertyChanged -= this.ItemPropertyChanged;
                        this.OnLayerChanged(item.Bounds);
                    }

                    foreach (var i in e.NewItems)
                    {
                        ImageLayerCollection.Item item = (ImageLayerCollection.Item)i;
                        item.PropertyChanged += this.ItemPropertyChanged;
                        this.OnLayerChanged(item.Bounds);
                    }

                    break;
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ImageLayerCollection.Item i = (ImageLayerCollection.Item)sender;
            this.OnLayerChanged(i.Bounds);
        }

        private void InvalidateSelectionRect(ImageLayerCollection.Item item)
        {
            var r = item.Bounds;
            this.OnLayerChanged(new Rectangle(r.Left, r.Top, r.Width, 1));
            this.OnLayerChanged(new Rectangle(r.Left, r.Top, 1, r.Height));
            this.OnLayerChanged(new Rectangle(r.Right, r.Top, 1, r.Height));
            this.OnLayerChanged(new Rectangle(r.Left, r.Bottom, r.Width + 1, 1));
        }
    }
}
