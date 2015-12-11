namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.Data;
    using Mappy.Database;

    public interface IMainFormViewModel
    {
        IObservable<IFeatureDatabase> FeatureRecords { get; }

        IObservable<IList<Section>> Sections { get; }

        IObservable<bool> CanSave { get; }

        IObservable<bool> CanSaveAs { get; }

        IObservable<bool> CanCloseMap { get; }

        IObservable<bool> CanImportMinimap { get; }

        IObservable<bool> CanExportMinimap { get; }

        IObservable<bool> CanImportHeightmap { get; }

        IObservable<bool> CanExportHeightmap { get; }

        IObservable<bool> CanImportCustomSection { get; }

        IObservable<bool> CanExportMapImage { get; }

        IObservable<bool> CanUndo { get; }

        IObservable<bool> CanRedo { get; }

        IObservable<bool> CanCut { get; }

        IObservable<bool> CanCopy { get; }

        IObservable<bool> CanPaste { get; }

        IObservable<bool> CanGenerateMinimap { get; }

        IObservable<bool> CanGenerateMinimapHighQuality { get; }

        IObservable<bool> CanOpenAttributes { get; }

        IObservable<bool> GridVisible { get; }

        IObservable<Size> GridSize { get; }

        IObservable<bool> HeightmapVisible { get; }

        IObservable<bool> FeaturesVisible { get; }

        IObservable<bool> MinimapVisible { get; }

        IObservable<bool> CanChangeSeaLevel { get; }

        IObservable<int> SeaLevel { get; }

        IObservable<string> TitleText { get; }

        void ToggleHeightmap();

        void ToggleMinimap();

        void ToggleFeatures();

        void OpenPreferences();

        void ShowAbout();

        void OpenMapAttributes();

        void ChooseColor();

        bool New();

        bool Open();

        bool OpenFromDragDrop(string filename);

        bool Save();

        bool SaveAs();

        void CloseMap();

        void Undo();

        void Redo();

        void Close();

        void RefreshMinimap();

        void RefreshMinimapHighQualityWithProgress();

        void HideGrid();

        void EnableGridWithSize(Size s);

        void SetSeaLevel(int value);

        void FlushSeaLevel();

        void CopySelectionToClipboard();

        void CutSelectionToClipboard();

        void PasteFromClipboard();

        void ImportMinimap();

        void ExportMinimap();

        void ImportHeightmap();

        void ExportHeightmap();

        void ExportMapImage();

        void ImportCustomSection();

        void Initialize();
    }
}
