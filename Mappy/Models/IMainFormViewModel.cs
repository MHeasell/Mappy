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

        void ToggleHeightMapMenuItemClick();

        void ToggleMinimapMenuItemClick();

        void ToggleFeaturesMenuItemClick();

        void PreferencesMenuItemClick();

        void AboutMenuItemClick();

        void MapAttributesMenuItemClick();

        void GridColorMenuItemClick();

        bool NewMenuItemClick();

        bool OpenMenuItemClick();

        bool DragDropFile(string filename);

        bool SaveMenuItemClick();

        bool SaveAsMenuItemClick();

        void CloseMenuItemClick();

        void UndoMenuItemClick();

        void RedoMenuItemClick();

        void FormCloseButtonClick();

        void ExitMenuItemClick();

        void GenerateMinimapMenuItemClick();

        void GenerateMinimapHighQualityMenuItemClick();

        void GridOffMenuItemClick();

        void GridMenuItemClick(Size s);

        void SeaLevelTrackBarValueChanged(int value);

        void SeaLevelTrackBarMouseUp();

        void CopyMenuItemClick();

        void CutMenuItemClick();

        void PasteMenuItemClick();

        void ImportMinimapMenuItemClick();

        void ExportMinimapMenuItemClick();

        void ImportHeightmapMenuItemClick();

        void ExportHeightmapMenuItemClick();

        void ExportMapImageMenuItemClick();

        void ImportCustomSectionMenuItemClick();

        void Load();
    }
}
