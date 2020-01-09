namespace Mappy.Services
{

    using System;
    using System.Linq;
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

        /// <summary>
        /// Search through existing features for the first that matches the specified coordinates.
        /// </summary>
        /// <param name="x">Look for a feature with this X location</param>
        /// <param name="y">Look for a feature with this Y location</param>
        /// <param name="divideCoords">Divides the given coordinates by 32 respectively, rounding down.</param>
        /// <returns>Returns the first(!) feature that has the specified coordinates or null if none are found.</returns>
        public DrawableItem GetItemWithCoords(int x, int y, bool divideCoords = false)
        {
            if (divideCoords)
            {
                x = x / 32;
                y = y / 32;
            }

            return this.Items.Where(i => i.X == x && i.Y == y).FirstOrDefault();
        }
    }
}
