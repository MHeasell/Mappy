namespace Mappy.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    using Geometry;

    using Mappy.Data;
    using Mappy.Database;

    public interface IMainFormModel : INotifyPropertyChanged
    {
        IFeatureDatabase FeatureRecords { get; }

        IList<Section> Sections { get; }

        bool CanUndo { get; }

        bool CanRedo { get; }

        bool CanCut { get; }

        bool CanCopy { get; }

        bool CanPaste { get; }

        bool IsDirty { get; }

        bool MapOpen { get; }

        string FilePath { get; }

        bool IsFileOpen { get; }

        bool IsFileReadOnly { get; }

        bool GridVisible { get; set; }

        Size GridSize { get; set; }

        bool HeightmapVisible { get; }

        bool FeaturesVisible { get; }

        bool MinimapVisible { get; }

        int MapWidth { get; }

        int MapHeight { get; }

        int SeaLevel { get; }

        Rectangle2D ViewportRectangle { get; set; }

        void Initialize();

        void SetGridSize(int size);

        void ChooseColor();

        void OpenMapAttributes();

        void Undo();

        void Redo();

        bool New();

        bool Save();

        bool SaveAs();

        bool Open();

        void Close();

        void ShowAbout();

        void ToggleHeightmap();

        void ToggleMinimap();

        void ToggleFeatures();

        void RefreshMinimap();

        void RefreshMinimapHighQualityWithProgress();

        void OpenPreferences();

        void CloseMap();

        void SetSeaLevel(int value);

        void FlushSeaLevel();

        void CutSelectionToClipboard();

        void CopySelectionToClipboard();

        void PasteFromClipboard();

        void ExportMinimap();

        void ExportHeightmap();

        void ImportMinimap();

        void ImportHeightmap();

        void ImportCustomSection();

        void ExportMapImage();
    }
}
