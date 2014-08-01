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

        bool MapOpen { get; }

        bool GridVisible { get; }

        Color GridColor { get; }

        Size GridSize { get; }

        bool HeightmapVisible { get; }

        bool FeaturesVisible { get; }

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }

        Rectangle BandboxRectangle { get; }

        ICollection<GridCoordinates> SelectedFeatures { get; }

        ISparseGrid<Feature> Features { get; }

        IList<Positioned<IMapTile>> FloatingTiles { get; }

        IMapTile BaseTile { get; }

        int MapWidth { get; }

        int MapHeight { get; }

        void DragDropStartPosition(int index, int x, int y);

        void DragDropTile(int id, int x, int y);

        void DragDropFeature(string name, int x, int y);

        void StartBandbox(int x, int y);

        void GrowBandbox(int x, int y);

        void CommitBandbox();

        void TranslateSelection(int x, int y);

        void FlushTranslation();

        void ClearSelection();

        void DeleteSelection();

        Point? GetStartPosition(int index);

        void SelectTile(int index);

        void SelectFeature(GridCoordinates index);

        void SelectStartPosition(int index);
    }
}