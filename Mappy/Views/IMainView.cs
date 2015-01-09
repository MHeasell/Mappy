namespace Mappy.Views
{
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.Data;

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

        void SetViewportPosition(int x, int y);
    }
}
