namespace Mappy.Models
{
    using System;
    using System.Drawing;

    public interface IMainFormViewModel
    {
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

        IObservable<bool> CanFill { get; }

        IObservable<bool> CanGenerateMinimap { get; }

        IObservable<bool> CanGenerateMinimapHighQuality { get; }

        IObservable<bool> CanOpenAttributes { get; }

        IObservable<bool> GridVisible { get; }

        IObservable<Size> GridSize { get; }

        IObservable<bool> HeightmapVisible { get; }

        IObservable<bool> HeightGridVisible { get; }

        IObservable<bool> VoidsVisible { get; }

        IObservable<bool> FeaturesVisible { get; }

        IObservable<bool> MinimapVisible { get; }

        IObservable<bool> CanChangeSeaLevel { get; }

        IObservable<int> SeaLevel { get; }

        IObservable<string> TitleText { get; }

        IObservable<string> MousePositionText { get; }

        IObservable<string> HoveredFeatureText { get; }

        void ToggleHeightMapMenuItemClick();

        void ToggleHeightGridMenuItemClick();

        void ToggleVoidsMenuItemClick();

        void ToggleMinimapMenuItemClick();

        void ToggleFeaturesMenuItemClick();

        void PreferencesMenuItemClick();

        void AboutMenuItemClick();

        void MapAttributesMenuItemClick();

        void GridColorMenuItemClick();

        void NewMenuItemClick();

        void OpenMenuItemClick();

        void DragDropFile(string filename);

        void SaveMenuItemClick();

        void SaveAsMenuItemClick();

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

        void FillMenuItemClick();

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
