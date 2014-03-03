namespace Mappy.Models
{
    using System.ComponentModel;

    public interface IMapSelectionModel : INotifyPropertyChanged
    {
        bool HasSelection { get; }

        int? SelectedFeature { get; }

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }

        void SelectAtPoint(int x, int y);

        void ClearSelection();

        void DeleteSelection();

        void TranslateSelection(int x, int y);

        void FlushTranslation();
    }
}
