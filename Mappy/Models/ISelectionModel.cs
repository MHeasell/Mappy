namespace Mappy.Models
{
    using System;
    using System.Collections.ObjectModel;

    public interface ISelectionModel : IBindingMapModel
    {
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
