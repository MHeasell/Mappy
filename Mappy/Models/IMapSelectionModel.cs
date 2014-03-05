namespace Mappy.Models
{
    using System.ComponentModel;

    public interface IMapSelectionModel : INotifyPropertyChanged
    {
        bool HasSelection { get; }

        int? SelectedFeature { get; }

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }
    }
}
