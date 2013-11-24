namespace Mappy.UI.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Painters;

    public partial class ImageLayerView : Control
    {
        private readonly ImageLayerCollection items = new ImageLayerCollection();

        private ImageLayerCollection.Item selectedItem;

        private bool gridVisible;
        private Color gridColor = Color.Black;
        private Size gridSize = new Size(16, 16);

        public ImageLayerView()
        {
            this.InitializeComponent();
            this.DoubleBuffered = true;
            this.Items.CollectionChanged += this.ItemsChanged;
        }

        public event EventHandler SelectedItemChanged;

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

        public ImageLayerCollection.Item SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                if (value != this.selectedItem)
                {
                    this.selectedItem = value;
                    this.OnSelectedItemChanged();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            foreach (var i in this.Items.EnumerateIntersecting(pe.ClipRectangle))
            {
                i.Draw(pe.Graphics, pe.ClipRectangle);
            }

            if (this.GridVisible)
            {
                using (Pen pen = new Pen(this.GridColor))
                {
                    GridPainter p = new GridPainter(this.GridSize.Width, pen);
                    p.Paint(pe.Graphics, pe.ClipRectangle);
                }
            }

            if (this.SelectedItem != null)
            {
                pe.Graphics.DrawRectangle(
                    Pens.Red,
                    this.SelectedItem.X,
                    this.SelectedItem.Y,
                    this.SelectedItem.Drawable.Width,
                    this.SelectedItem.Drawable.Height);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.SelectedItem = this.Items.HitTest(e.Location);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            this.SelectedItem = null;
        }

        protected virtual void OnSelectedItemChanged()
        {
            this.Invalidate();

            EventHandler h = this.SelectedItemChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }

        private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var i in e.NewItems)
                    {
                        ((ImageLayerCollection.Item)i).PropertyChanged += this.ItemPropertyChanged;
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var i in e.OldItems)
                    {
                        ((ImageLayerCollection.Item)i).PropertyChanged -= this.ItemPropertyChanged;
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var i in e.OldItems)
                    {
                        ((ImageLayerCollection.Item)i).PropertyChanged -= this.ItemPropertyChanged;
                    }

                    foreach (var i in e.NewItems)
                    {
                        ((ImageLayerCollection.Item)i).PropertyChanged += this.ItemPropertyChanged;
                    }

                    break;
            }

            this.Invalidate();
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ImageLayerCollection.Item i = (ImageLayerCollection.Item)sender;

            if (i == this.SelectedItem && !i.Visible)
            {
                this.SelectedItem = null;
            }

            this.Invalidate();
        }
    }
}
