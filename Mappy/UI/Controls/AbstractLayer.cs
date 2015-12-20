namespace Mappy.UI.Controls
{
    using System;
    using System.Drawing;

    public abstract class AbstractLayer : ILayer
    {
        private bool enabled = true;

        public event EventHandler<LayerChangedEventArgs> LayerChanged;

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }

            set
            {
                if (value != this.enabled)
                {
                    this.enabled = value;
                    this.LayerChanged?.Invoke(this, new LayerChangedEventArgs());
                }
            }
        }

        public void Draw(Graphics graphics, Rectangle clipRectangle)
        {
            if (this.enabled)
            {
                this.DoDraw(graphics, clipRectangle);
            }
        }

        protected abstract void DoDraw(Graphics graphics, Rectangle clipRectangle);

        protected void OnLayerChanged()
        {
            this.OnLayerChanged(new LayerChangedEventArgs());
        }

        protected void OnLayerChanged(Rectangle rect)
        {
            this.OnLayerChanged(new LayerChangedEventArgs(rect));
        }

        protected virtual void OnLayerChanged(LayerChangedEventArgs e)
        {
            if (this.enabled)
            {
                this.LayerChanged?.Invoke(this, e);
            }
        }
    }
}
