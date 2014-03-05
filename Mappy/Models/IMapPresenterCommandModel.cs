namespace Mappy.Models
{
    using System.Drawing;
    using Mappy.Data;

    public interface IMapPresenterCommandModel
    {
        IBindingMapModel Map { get; }

        int PlaceSection(int tileId, int x, int y);

        void TranslateSection(Positioned<IMapTile> tile, int x, int y);

        void TranslateSection(int index, int x, int y);

        bool TranslateFeature(Point featureCoord, int x, int y);

        bool TranslateFeature(int index, int x, int y);

        void FlushTranslation();

        bool TryPlaceFeature(string name, int x, int y);

        void RemoveSection(int index);

        void RemoveFeature(int index);

        void RemoveFeature(int x, int y);

        void RemoveFeature(Point coords);

        void SetStartPosition(int i, int x, int y);

        void TranslateStartPosition(int i, int x, int y);

        void TranslateStartPositionTo(int i, int x, int y);

        void RemoveStartPosition(int i);
    }
}
