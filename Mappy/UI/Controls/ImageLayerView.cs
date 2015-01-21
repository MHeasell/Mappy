namespace Mappy.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.UI.Painters;

    public class ImageLayerView : ScrollableControl
    {
        private readonly ImageLayerCollection items;

        private readonly List<ImageLayerCollection.Item> selectedItems = new List<ImageLayerCollection.Item>();

        private Size canvasSize;

        private Color canvasColor = Color.CornflowerBlue;

        private Brush canvasBrush;

        private bool gridVisible;
        private Color gridColor = Color.Black;
        private Size gridSize = new Size(16, 16);

        public ImageLayerView()
        {
            this.canvasBrush = new SolidBrush(this.canvasColor);

            this.DoubleBuffered = true;
            this.items = new ImageLayerCollection(this.Width, this.Height);
            this.Items.CollectionChanged += this.ItemsChanged;
        }

        public event EventHandler CanvasSizeChanged;

        public event EventHandler CanvasColorChanged;

        public bool GridVisible
        {
            get
            {
                return this.gridVisible;
            }

            set
            {
                this.gridVisible = value;
                this.Invalidate();
            }
        }

        public Size GridSize
        {
            get
            {
                return this.gridSize;
            }

            set
            {
                this.gridSize = value;
                this.Invalidate();
            }
        }

        public Color GridColor
        {
            get
            {
                return this.gridColor;
            }

            set
            {
                this.gridColor = value;
                this.Invalidate();
            }
        }

        public ImageLayerCollection Items
        {
            get { return this.items; }
        }

        public Size CanvasSize
        {
            get
            {
                return this.canvasSize;
            }

            set
            {
                if (value != this.canvasSize)
                {
                    this.canvasSize = value;
                    this.OnCanvasSizeChanged();
                }
            }
        }

        public Color CanvasColor
        {
            get
            {
                return this.canvasColor;
            }

            set
            {
                if (value != this.canvasColor)
                {
                    this.canvasColor = value;
                    this.OnCanvasColorChanged();
                }
            }
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

        public Point ToVirtualPoint(Point clientPoint)
        {
            return new Point(
                clientPoint.X - this.AutoScrollPosition.X,
                clientPoint.Y - this.AutoScrollPosition.Y);
        }

        public Rectangle ToClientRect(Rectangle rect)
        {
            Rectangle outRect = rect;
            outRect.Offset(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            return outRect;
        }

        public Rectangle ToVirtualRect(Rectangle clientRect)
        {
            Rectangle outRect = clientRect;
            outRect.Offset(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y);
            return outRect;
        }

        protected virtual void OnCanvasSizeChanged()
        {
            this.Items.Resize(this.CanvasSize.Width, this.CanvasSize.Height);

            this.AutoScrollMinSize = this.CanvasSize;
            this.Invalidate();

            EventHandler h = this.CanvasSizeChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            // paint canvas background
            Rectangle canvasArea = this.ToClientRect(new Rectangle(Point.Empty, this.CanvasSize));
            canvasArea.Intersect(pe.ClipRectangle);
            pe.Graphics.FillRectangle(this.canvasBrush, canvasArea);

            // translate to virtual coordinates
            pe.Graphics.TranslateTransform(
                this.AutoScrollPosition.X,
                this.AutoScrollPosition.Y);

            // virtual clip rect
            Rectangle virtualClip = this.ToVirtualRect(pe.ClipRectangle);

            // draw items
            foreach (var i in this.Items.EnumerateIntersecting(virtualClip))
            {
                i.Draw(pe.Graphics, virtualClip);
            }

            // draw grid
            if (this.GridVisible)
            {
                using (Pen pen = new Pen(this.GridColor))
                {
                    GridPainter p = new GridPainter(this.GridSize.Width, pen);
                    p.Paint(pe.Graphics, virtualClip);
                }
            }

            // draw guides
            using (var guidePen = new Pen(Color.Magenta, 3.0f))
            {
                var horizLinePos = this.CanvasSize.Height - 128;
                pe.Graphics.DrawLine(guidePen, 0, horizLinePos, this.CanvasSize.Width, horizLinePos);

                int vertLinePos = this.CanvasSize.Width - 32;
                pe.Graphics.DrawLine(guidePen, vertLinePos, 0, vertLinePos, this.CanvasSize.Height);
            }

            // draw selection rectangles
            foreach (var i in this.selectedItems)
            {
                pe.Graphics.DrawRectangle(
                    Pens.Red,
                    i.X,
                    i.Y,
                    i.Drawable.Width,
                    i.Drawable.Height);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();

            base.OnMouseDown(e);
        }

        protected virtual void OnCanvasColorChanged()
        {
            this.canvasBrush.Dispose();
            this.canvasBrush = new SolidBrush(this.CanvasColor);
            this.Invalidate();

            EventHandler h = this.CanvasColorChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.canvasBrush.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InvalidateSelectionRect(ImageLayerCollection.Item item)
        {
            Rectangle r = this.ToClientRect(item.Bounds);
            this.Invalidate(new Rectangle(r.Left, r.Top, r.Width, 1));
            this.Invalidate(new Rectangle(r.Left, r.Top, 1, r.Height));
            this.Invalidate(new Rectangle(r.Right, r.Top, 1, r.Height));
            this.Invalidate(new Rectangle(r.Left, r.Bottom, r.Width + 1, 1));
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
                        this.Invalidate(this.ToClientRect(item.Bounds));
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var i in e.OldItems)
                    {
                        ImageLayerCollection.Item item = (ImageLayerCollection.Item)i;
                        item.PropertyChanged -= this.ItemPropertyChanged;

                        this.Invalidate(this.ToClientRect(item.Bounds));
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var i in e.OldItems)
                    {
                        ImageLayerCollection.Item item = (ImageLayerCollection.Item)i;
                        item.PropertyChanged -= this.ItemPropertyChanged;

                        this.Invalidate(this.ToClientRect(item.Bounds));
                    }

                    foreach (var i in e.NewItems)
                    {
                        ImageLayerCollection.Item item = (ImageLayerCollection.Item)i;
                        item.PropertyChanged += this.ItemPropertyChanged;
                        this.Invalidate(this.ToClientRect(item.Bounds));
                    }

                    break;
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ImageLayerCollection.Item i = (ImageLayerCollection.Item)sender;

            this.Invalidate(this.ToClientRect(i.Bounds));
        }
    }
}
