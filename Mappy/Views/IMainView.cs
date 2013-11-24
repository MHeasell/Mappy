namespace Mappy.Views
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Controllers;
    using Data;
    using Models;

    public interface IMainView
    {
        event EventHandler ViewportLocationChanged;
        
        MainPresenter Presenter { get; set; }

        string TitleText { get; set; }

        bool SaveEnabled { get; set; }

        bool SaveAsEnabled { get; set; }

        bool UndoEnabled { get; set; }

        bool RedoEnabled { get; set; }

        bool MinimapVisible { get; set; }

        bool OpenAttributesEnabled { get; set; }

        IBindingMapModel Map { get; set; }

        IList<Section> Sections { get; set; }

        IList<Feature> Features { get; set; }

        Rectangle ViewportRect { get; }

        void SetViewportCenter(Point p);

        string AskUserToChooseMap(IList<string> maps);

        string AskUserToOpenFile();

        string AskUserToSaveFile();

        void CapturePreferences();

        Size AskUserNewMapSize();

        Color? AskUserGridColor(Color previousColor);

        DialogResult AskUserToDiscardChanges();

        MapAttributesResult AskUserForMapAttributes(MapAttributesResult r);

        void Close();
    }
}
