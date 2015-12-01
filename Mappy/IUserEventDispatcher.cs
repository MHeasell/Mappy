namespace Mappy
{
    using System;
    using System.Drawing;

    using Mappy.Data;
    using Mappy.Models;

    public interface IUserEventDispatcher
    {
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

        void SetViewportSize(Size size);

        void SetViewportLocation(Point location);

        void HideMinimap();
    }
}
