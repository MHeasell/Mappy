namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Data;

    public interface IMainModel : INotifyPropertyChanged
    {
        event EventHandler<ListChangedEventArgs> TilesChanged;

        event EventHandler<GridEventArgs> BaseTileGraphicsChanged;

        event EventHandler<GridEventArgs> BaseTileHeightChanged;

        event EventHandler<FeatureInstanceEventArgs> FeatureInstanceChanged;

        event EventHandler<StartPositionChangedEventArgs> StartPositionChanged;

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }

        ObservableCollection<Guid> SelectedFeatures { get; }

        Rectangle BandboxRectangle { get; }

        IList<Positioned<IMapTile>> FloatingTiles { get; }

        IMapTile BaseTile { get; }

        int MapWidth { get; }

        int MapHeight { get; }

        int FeatureGridWidth { get; }

        int FeatureGridHeight { get; }

        int SeaLevel { get; }

        Point ViewportLocation { get; set; }

        FeatureInstance GetFeatureInstance(Guid id);

        IEnumerable<FeatureInstance> EnumerateFeatureInstances(); 

        void DragDropStartPosition(int index, int x, int y);

        void DragDropTile(IMapTile tile, int x, int y);

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

        void SelectFeature(Guid id);

        void SelectStartPosition(int index);
    }
}