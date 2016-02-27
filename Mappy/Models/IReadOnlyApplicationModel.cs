namespace Mappy.Models
{
    using System.ComponentModel;
    using System.Drawing;

    public interface IReadOnlyApplicationModel : INotifyPropertyChanged
    {
        UndoableMapModel Map { get; }

        bool CanUndo { get; }

        bool CanRedo { get; }

        bool CanCopy { get; }

        bool CanPaste { get; }

        bool CanCut { get; }

        bool HeightmapVisible { get; }

        bool FeaturesVisible { get; }

        int ViewportWidth { get; }

        int ViewportHeight { get; }

        bool MinimapVisible { get; }

        bool GridVisible { get; }

        Size GridSize { get; }

        Color GridColor { get; }
    }
}