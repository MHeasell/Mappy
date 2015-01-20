namespace Mappy.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Mappy.Collections;

    public interface ISelectionModel : IMapModel, INotifyPropertyChanged
    {
        event EventHandler<FeatureInstanceEventArgs> FeatureInstanceChanged;

        event EventHandler<GridEventArgs> TileGridChanged;

        event EventHandler<GridEventArgs> HeightGridChanged;

        event ListChangedEventHandler FloatingTilesChanged;

        event EventHandler<SparseGridEventArgs> VoidsChanged;

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }

        ObservableCollection<Guid> SelectedFeatures { get; }

        void SelectTile(int index);

        void DeselectTile();

        void TranslateSelectedTile(int x, int y);

        void DeleteSelectedTile();

        void MergeSelectedTile();

        void SelectFeature(Guid id);

        void DeselectFeature(Guid id);

        void DeselectFeatures();

        void DeletedSelectedFeatures();

        void SelectStartPosition(int index);

        void DeselectStartPosition();

        void TranslateSelectedStartPosition(int x, int y);

        void DeleteSelectedStartPosition();

        void DeselectAll();
    }
}
