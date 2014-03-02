namespace Mappy.Models
{
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Data;

    public interface IMapPresenterModel : INotifyPropertyChanged
    {
        IBindingMapModel Map { get; }

        bool HeightmapVisible { get; set; }

        bool FeaturesVisible { get; set; }

        bool GridVisible { get; set; }

        Size GridSize { get; set; }

        Color GridColor { get; set; }

        void PlaceSection(int tileId, int x, int y);

        void TranslateSection(Positioned<IMapTile> tile, int x, int y);

        bool TranslateFeature(Point featureCoord, int x, int y);

        void FlushTranslation();

        bool TryPlaceFeature(string name, int x, int y);

        void RemoveSection(int index);

        void RemoveFeature(int x, int y);

        void RemoveFeature(Point coords);

        void SetStartPosition(int i, int x, int y);

        void TranslateStartPositionTo(int i, int x, int y);

        void RemoveStartPosition(int i);
    }
}