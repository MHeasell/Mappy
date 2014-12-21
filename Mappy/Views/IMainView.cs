namespace Mappy.Views
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Data;
    using Mappy.Models;

    public interface IMainView
    {
        string TitleText { get; set; }

        bool SaveEnabled { get; set; }

        bool SaveAsEnabled { get; set; }

        bool UndoEnabled { get; set; }

        bool RedoEnabled { get; set; }

        bool CutEnabled { get; set; }

        bool CopyEnabled { get; set; }

        bool PasteEnabled { get; set; }

        bool SeaLevelEditEnabled { get; set; }

        bool OpenAttributesEnabled { get; set; }

        bool MinimapVisibleChecked { get; set; }

        bool RefreshMinimapEnabled { get; set; }

        bool RefreshMinimapHighQualityEnabled { get; set; }

        int SeaLevel { get; set; }

        Rectangle ViewportRect { get; }

        IList<Section> Sections { get; set; }

        IList<Feature> Features { get; set; }

        bool CloseEnabled { get; set; }

        bool ImportMinimapEnabled { get; set; }

        bool ExportMinimapEnabled { get; set; }

        bool ExportHeightmapEnabled { get; set; }

        bool ImportHeightmapEnabled { get; set; }

        bool ExportMapImageEnabled { get; set; }

        bool ImportCustomSectionEnabled { get; set; }

        string AskUserToChooseMap(IList<string> maps);

        string AskUserToOpenFile();

        string AskUserToSaveFile();

        string AskUserToSaveMinimap();

        string AskUserToSaveMapImage();

        string AskUserToChooseMinimap();

        string AskUserToSaveHeightmap();

        string AskUserToChooseHeightmap(int width, int height);

        SectionImportPaths AskUserToChooseSectionImportPaths();

        void CapturePreferences();

        Size AskUserNewMapSize();

        Color? AskUserGridColor(Color previousColor);

        DialogResult AskUserToDiscardChanges();

        MapAttributesResult AskUserForMapAttributes(MapAttributesResult r);

        void Close();

        void ShowError(string message);

        IProgressView CreateProgressView();

        void SetViewportPosition(int x, int y);
    }

    public class SectionImportPaths
    {
        public string GraphicPath { get; set; }

        public string HeightmapPath { get; set; }
    }
}
