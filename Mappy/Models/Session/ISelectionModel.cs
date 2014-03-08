namespace Mappy.Models.Session
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Collections;

    public interface ISelectionModel : INotifyPropertyChanged
    {
        bool HasSelection { get; }

        ICollection<GridCoordinates> SelectedFeatures { get; }

        int? SelectedTile { get; }

        int? SelectedStartPosition { get; }

        Rectangle BandboxRectangle { get; }
    }
}
