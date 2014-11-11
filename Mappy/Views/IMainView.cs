namespace Mappy.Views
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Data;
    using Models;

    public interface IMainView
    {
        string TitleText { get; set; }

        bool SaveEnabled { get; set; }

        bool SaveAsEnabled { get; set; }

        bool UndoEnabled { get; set; }

        bool RedoEnabled { get; set; }

        bool CopyEnabled { get; set; }

        bool PasteEnabled { get; set; }

        bool SeaLevelEditEnabled { get; set; }

        bool OpenAttributesEnabled { get; set; }

        bool MinimapVisibleChecked { get; set; }

        int SeaLevel { get; set; }

        Rectangle ViewportRect { get; }

        IList<Section> Sections { get; set; }

        IList<Feature> Features { get; set; }

        bool CloseEnabled { get; set; }

        string AskUserToChooseMap(IList<string> maps);

        string AskUserToOpenFile();

        string AskUserToSaveFile();

        void CapturePreferences();

        Size AskUserNewMapSize();

        Color? AskUserGridColor(Color previousColor);

        DialogResult AskUserToDiscardChanges();

        MapAttributesResult AskUserForMapAttributes(MapAttributesResult r);

        void Close();

        void ShowError(string message);
    }
}
