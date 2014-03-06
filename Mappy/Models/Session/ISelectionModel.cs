namespace Mappy.Models.Session
{
    using System.ComponentModel;
    using System.Drawing;

    public interface ISelectionModel : INotifyPropertyChanged
    {
        bool HasSelection { get; }

        Point? SelectedFeature { get; }

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }
    }
}
