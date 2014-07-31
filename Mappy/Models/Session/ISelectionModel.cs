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

        void ClearSelection();

        void DeleteSelection();

        void TranslateSelection(int x, int y);

        void FlushTranslation();

        void DragDropFeature(string name, int x, int y);

        void DragDropTile(int id, int x, int y);

        void DragDropStartPosition(int index, int x, int y);

        void StartBandbox(int x, int y);

        void GrowBandbox(int x, int y);

        void CommitBandbox();

        void SelectTile(int index);
    }
}
