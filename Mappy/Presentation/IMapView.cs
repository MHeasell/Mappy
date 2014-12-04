namespace Mappy.Presentation
{
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.UI.Controls;

    public interface IMapView
    {
        bool GridVisible { get; set; }

        Color GridColor { get; set; }

        Size GridSize { get; set; }

        Size CanvasSize { get; set; }

        ICollection<ImageLayerCollection.Item> Items { get; }

        bool IsInSelection(int x, int y);

        ImageLayerCollection.Item HitTest(int x, int y);

        void RemoveFromSelection(ImageLayerCollection.Item item);

        bool SelectedItemsContains(ImageLayerCollection.Item item);

        void AddToSelection(ImageLayerCollection.Item item);

        void Invalidate();

        void ClearSelection();
    }
}
