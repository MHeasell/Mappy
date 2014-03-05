namespace Mappy.Models
{
    using System.ComponentModel;
    using System.Drawing;

    public interface IMapSelectionModel : INotifyPropertyChanged
    {
        bool HasSelection { get; }

        Point? SelectedFeature { get; }

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }
    }
}
