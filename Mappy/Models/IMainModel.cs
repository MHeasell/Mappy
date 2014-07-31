namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Data;

    public interface IMainModel : INotifyPropertyChanged
    {
        event EventHandler<SparseGridEventArgs> FeaturesChanged;

        event EventHandler<ListChangedEventArgs> TilesChanged;

        event EventHandler<GridEventArgs> BaseTileGraphicsChanged;

        event EventHandler<GridEventArgs> BaseTileHeightChanged;

        event EventHandler<StartPositionChangedEventArgs> StartPositionChanged;

        bool GridVisible { get; }

        Color GridColor { get; }

        Size GridSize { get; }

        bool HeightmapVisible { get; }

        bool FeaturesVisible { get; }

        ISparseGrid<Feature> Features { get; }

        IList<Positioned<IMapTile>> FloatingTiles { get; }

        IMapTile BaseTile { get; }

        int MapWidth { get; }

        int MapHeight { get; }

        bool MapOpen { get; }

        Point? GetStartPosition(int index);

        int PlaceSection(int tileId, int x, int y);

        void TranslateSection(int index, int x, int y);

        bool TranslateFeatureBatch(IEnumerable<GridCoordinates> coords, int x, int y);

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

        void RemoveStartPosition(int i);

        void MergeSection(int index);

        int LiftArea(int x, int y, int width, int height);

        Point? ScreenToHeightIndex(int x, int y);
    }
}
