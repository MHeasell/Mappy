namespace Mappy.Data
{
    using System;
    using System.Drawing;

    public class Positioned<T>
    {
        private Point location;

        public Positioned(T item)
            : this(item, new Point())
        {
        }

        public Positioned(T item, Point location)
        {
            this.Item = item;
            this.Location = location;
        }

        public event EventHandler LocationChanged;

        public Point Location
        {
            get
            {
                return this.location;
            }

            set
            {
                this.location = value;
                this.OnLocationChanged();
            }
        }

        public T Item { get; private set; }

        private void OnLocationChanged()
        {
            EventHandler h = this.LocationChanged;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }
    }
}
