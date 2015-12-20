namespace Mappy.UI.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;

    // An ordered collection of layers.
    // Provides events to notify when an area changes
    // or when the whole canvas becomes invalid.
    public class LayerCollection : IList<ILayer>
    {
        private readonly List<ILayer> items = new List<ILayer>();

        public event EventHandler FullRedraw;

        public event EventHandler<AreaChangedEventArgs> AreaChanged;

        public ILayer this[int index]
        {
            get
            {
                return this.items[index];
            }

            set
            {
                this.items[index].LayerChanged -= this.OnLayerAreaChanged;
                this.items[index] = value;
                this.items[index].LayerChanged += this.OnLayerAreaChanged;

                this.OnFullRedraw();
            }
        }

        public IEnumerator<ILayer> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        public void Add(ILayer item)
        {
            this.items.Add(item);

            item.LayerChanged += this.OnLayerAreaChanged;

            this.OnFullRedraw();
        }

        public void Clear()
        {
            foreach (var item in this.items)
            {
                item.LayerChanged -= this.OnLayerAreaChanged;
            }

            this.items.Clear();

            this.OnFullRedraw();
        }

        public bool Contains(ILayer item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(ILayer[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        public bool Remove(ILayer item)
        {
            var success = this.items.Remove(item);
            if (success)
            {
                item.LayerChanged -= this.OnLayerAreaChanged;
                this.OnFullRedraw();
            }

            return success;
        }

        public int Count => this.items.Count;

        public bool IsReadOnly => false;

        public int IndexOf(ILayer item)
        {
            return this.items.IndexOf(item);
        }

        public void Insert(int index, ILayer item)
        {
            this.items.Insert(index, item);
            item.LayerChanged += this.OnLayerAreaChanged;
            this.OnFullRedraw();
        }

        public void RemoveAt(int index)
        {
            var item = this.items[index];
            this.items.RemoveAt(index);
            item.LayerChanged -= this.OnLayerAreaChanged;
            this.OnFullRedraw();
        }

        public void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            foreach (var layer in this.items)
            {
                layer.Draw(graphics, clipRectangle);
            }
        }

        private void OnFullRedraw()
        {
            this.FullRedraw?.Invoke(this, EventArgs.Empty);
        }

        private void OnAreaChanged(Rectangle r)
        {
            this.OnAreaChanged(new AreaChangedEventArgs(r));
        }

        private void OnAreaChanged(AreaChangedEventArgs e)
        {
            this.AreaChanged?.Invoke(this, e);
        }

        private void OnLayerAreaChanged(object sender, LayerChangedEventArgs e)
        {
            if (e.AllChanged)
            {
                this.OnFullRedraw();
            }
            else
            {
                this.OnAreaChanged(e.ChangedRectangle);
            }
        }
    }
}
