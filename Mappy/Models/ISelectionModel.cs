namespace Mappy.Models
{
    using System;
    using System.Collections.ObjectModel;

    using Mappy.Collections;

    public interface ISelectionModel : IBindingMapModel
    {
        event EventHandler SelectedTileChanged;

        event EventHandler SelectedStartPositionChanged;

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }

        ObservableCollection<GridCoordinates> SelectedFeatures { get; }

        void SelectTile(int index);

        void DeselectTile();

        void TranslateSelectedTile(int x, int y);

        void DeleteSelectedTile();

        void MergeSelectedTile();

        void SelectFeature(GridCoordinates index);

        void DeselectFeature(GridCoordinates index);

        void DeselectFeatures();

        bool CanTranslateSelectedFeatures(int x, int y);

        void TranslateSelectedFeatures(int x, int y);

        void DeletedSelectedFeatures();

        void SelectStartPosition(int index);

        void DeselectStartPosition();

        void TranslateSelectedStartPosition(int x, int y);

        void DeleteSelectedStartPosition();

        void DeselectAll();
    }
}
