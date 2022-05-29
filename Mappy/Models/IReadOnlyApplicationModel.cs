namespace Mappy.Models
{
    using System.ComponentModel;
    using System.Drawing;
    using Mappy.Models.Enums;

    public interface IReadOnlyApplicationModel : INotifyPropertyChanged
    {
        Maybe<UndoableMapModel> Map { get; }

        bool HeightmapVisible { get; }

        bool HeightGridVisible { get; }

        bool VoidsVisible { get; }

        bool FeaturesVisible { get; }

        int ViewportWidth { get; }

        int ViewportHeight { get; }

        bool MinimapVisible { get; }

        bool GridVisible { get; }

        Size GridSize { get; }

        Color GridColor { get; }

        GUITab SelectedGUITab { get; }
    }
}