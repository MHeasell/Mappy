namespace Mappy.Services
{

    using System;
    using Mappy.UI.Controls;

    public class AccessibleFeatures : IObserver<ILayer>
    {
        public DrawableItemCollection Items { get; private set; }

        public void OnCompleted()
        {
            // throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(ILayer value)
        {
            if (value == null)
            {
                return;
            }

            SelectableItemsLayer selItemLayer = (SelectableItemsLayer)value;
            this.Items = selItemLayer.Items;
        }
    }
}
