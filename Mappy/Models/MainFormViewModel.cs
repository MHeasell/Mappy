﻿namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Reactive.Linq;

    using Mappy.Services;

    public class MainFormViewModel : IMainFormViewModel
    {
        private const string ProgramName = "Mappy";

        private readonly Dispatcher dispatcher;

        public MainFormViewModel(IReadOnlyApplicationModel model, Dispatcher dispatcher)
        {
            var map = model.PropertyAsObservable(x => x.Map, nameof(model.Map));
            var mapOpen = map.Select(x => x.IsSome);
            var isDirty = map.ObservePropertyOrDefault(x => x.IsMarked, "IsMarked", true).Select(x => !x);
            var filePath = map.ObservePropertyOrDefault(x => x.FilePath, "FilePath", null);
            var isFileReadOnly = map.ObservePropertyOrDefault(x => x.IsFileReadOnly, "IsFileReadOnly", false);

            this.CanUndo = model.PropertyAsObservable(x => x.CanUndo, "CanUndo");
            this.CanRedo = model.PropertyAsObservable(x => x.CanRedo, "CanRedo");
            this.CanCut = model.PropertyAsObservable(x => x.CanCut, "CanCut");
            this.CanCopy = model.PropertyAsObservable(x => x.CanCopy, "CanCopy");
            this.CanPaste = model.PropertyAsObservable(x => x.CanPaste, "CanPaste");
            this.CanFill = model.PropertyAsObservable(x => x.CanCopy, "CanFill");
            this.GridVisible = model.PropertyAsObservable(x => x.GridVisible, "GridVisible");
            this.GridSize = model.PropertyAsObservable(x => x.GridSize, "GridSize");
            this.HeightmapVisible = model.PropertyAsObservable(x => x.HeightmapVisible, "HeightmapVisible");
            this.VoidsVisible = model.PropertyAsObservable(x => x.VoidsVisible, nameof(model.VoidsVisible));
            this.FeaturesVisible = model.PropertyAsObservable(x => x.FeaturesVisible, "FeaturesVisible");
            this.MinimapVisible = model.PropertyAsObservable(x => x.MinimapVisible, "MinimapVisible");
            this.SeaLevel = map.ObservePropertyOrDefault(x => x.SeaLevel, "SeaLevel", 0);

            this.CanSaveAs = mapOpen;
            this.CanCloseMap = mapOpen;
            this.CanImportMinimap = mapOpen;
            this.CanExportMinimap = mapOpen;
            this.CanImportHeightmap = mapOpen;
            this.CanExportHeightmap = mapOpen;
            this.CanImportCustomSection = mapOpen;
            this.CanExportMapImage = mapOpen;
            this.CanGenerateMinimap = mapOpen;
            this.CanGenerateMinimapHighQuality = mapOpen;
            this.CanOpenAttributes = mapOpen;
            this.CanChangeSeaLevel = mapOpen;

            // set up CanSave observable
            var canSave = Observable.CombineLatest(
                mapOpen,
                filePath.Select(x => x != null),
                isFileReadOnly.Select(x => !x))
                .Select(x => x.All(y => y))
                .Replay(1);
            canSave.Connect();
            this.CanSave = canSave;

            // set up TitleText observable
            var cleanFileName = filePath.Select(x => (x ?? "Untitled"));
            var dirtyFileName = cleanFileName.Select(x => x + "*");

            var fileName = isDirty
                .Select(x => x ? dirtyFileName : cleanFileName)
                .Switch();
            var readOnlyFileName = fileName.Select(y => y + " [read only]");

            var fileNameTitle = isFileReadOnly
                .Select(x => x ? readOnlyFileName : fileName)
                .Switch();

            var defaultTitle = Observable.Return(ProgramName);
            var openFileTitle = fileNameTitle.Select(y => y + " - " + ProgramName);
            var titleText = mapOpen
                .Select(x => x ? openFileTitle : defaultTitle)
                .Switch()
                .Replay(1);
            titleText.Connect();

            this.TitleText = titleText;

            this.dispatcher = dispatcher;
        }

        public IObservable<bool> CanCloseMap { get; }

        public IObservable<bool> CanSave { get; }

        public IObservable<bool> CanSaveAs { get; }

        public IObservable<bool> CanImportMinimap { get; }

        public IObservable<bool> CanExportMinimap { get; }

        public IObservable<bool> CanImportHeightmap { get; }

        public IObservable<bool> CanExportHeightmap { get; }

        public IObservable<bool> CanImportCustomSection { get; }

        public IObservable<bool> CanExportMapImage { get; }

        public IObservable<bool> CanGenerateMinimap { get; }

        public IObservable<bool> CanGenerateMinimapHighQuality { get; }

        public IObservable<bool> CanOpenAttributes { get; }

        public IObservable<bool> CanChangeSeaLevel { get; }

        public IObservable<string> TitleText { get; }

        public IObservable<bool> CanUndo { get; }

        public IObservable<bool> CanRedo { get; }

        public IObservable<bool> CanCut { get; }

        public IObservable<bool> CanCopy { get; }

        public IObservable<bool> CanPaste { get; }

        public IObservable<bool> CanFill { get; }

        public IObservable<bool> GridVisible { get; }

        public IObservable<Size> GridSize { get; }

        public IObservable<bool> HeightmapVisible { get; }

        public IObservable<bool> VoidsVisible { get; }

        public IObservable<bool> FeaturesVisible { get; }

        public IObservable<bool> MinimapVisible { get; }

        public IObservable<int> SeaLevel { get; }

        public void ToggleHeightMapMenuItemClick()
        {
            this.dispatcher.ToggleHeightmap();
        }

        public void ToggleVoidsMenuItemClick()
        {
            this.dispatcher.ToggleVoids();
        }

        public void ToggleMinimapMenuItemClick()
        {
            this.dispatcher.ToggleMinimap();
        }

        public void ToggleFeaturesMenuItemClick()
        {
            this.dispatcher.ToggleFeatures();
        }

        public void PreferencesMenuItemClick()
        {
            this.dispatcher.OpenPreferences();
        }

        public void AboutMenuItemClick()
        {
            this.dispatcher.ShowAbout();
        }

        public void MapAttributesMenuItemClick()
        {
            this.dispatcher.OpenMapAttributes();
        }

        public void GridColorMenuItemClick()
        {
            this.dispatcher.ChooseColor();
        }

        public void NewMenuItemClick()
        {
            this.dispatcher.New();
        }

        public void OpenMenuItemClick()
        {
            this.dispatcher.Open();
        }

        public void DragDropFile(string filename)
        {
            this.dispatcher.OpenFromDragDrop(filename);
        }

        public void SaveMenuItemClick()
        {
            this.dispatcher.Save();
        }

        public void SaveAsMenuItemClick()
        {
            this.dispatcher.SaveAs();
        }

        public void CloseMenuItemClick()
        {
            this.dispatcher.CloseMap();
        }

        public void UndoMenuItemClick()
        {
            this.dispatcher.Undo();
        }

        public void RedoMenuItemClick()
        {
            this.dispatcher.Redo();
        }

        public void FormCloseButtonClick()
        {
            this.dispatcher.Close();
        }

        public void ExitMenuItemClick()
        {
            this.dispatcher.Close();
        }

        public void GenerateMinimapMenuItemClick()
        {
            this.dispatcher.RefreshMinimap();
        }

        public void GenerateMinimapHighQualityMenuItemClick()
        {
            this.dispatcher.RefreshMinimapHighQualityWithProgress();
        }

        public void GridOffMenuItemClick()
        {
            this.dispatcher.HideGrid();
        }

        public void GridMenuItemClick(Size s)
        {
            this.dispatcher.EnableGridWithSize(s);
        }

        public void SeaLevelTrackBarValueChanged(int value)
        {
            this.dispatcher.SetSeaLevel(value);
        }

        public void SeaLevelTrackBarMouseUp()
        {
            this.dispatcher.FlushSeaLevel();
        }

        public void CopyMenuItemClick()
        {
            this.dispatcher.CopySelectionToClipboard();
        }

        public void CutMenuItemClick()
        {
            this.dispatcher.CutSelectionToClipboard();
        }

        public void PasteMenuItemClick()
        {
            this.dispatcher.PasteFromClipboard();
        }

        public void FillMenuItemClick()
        {
            this.dispatcher.FillSelection();
        }

        public void ImportMinimapMenuItemClick()
        {
            this.dispatcher.ImportMinimap();
        }

        public void ExportMinimapMenuItemClick()
        {
            this.dispatcher.ExportMinimap();
        }

        public void ImportHeightmapMenuItemClick()
        {
            this.dispatcher.ImportHeightmap();
        }

        public void ExportHeightmapMenuItemClick()
        {
            this.dispatcher.ExportHeightmap();
        }

        public void ExportMapImageMenuItemClick()
        {
            this.dispatcher.ExportMapImage();
        }

        public void ImportCustomSectionMenuItemClick()
        {
            this.dispatcher.ImportCustomSection();
        }

        public void Load()
        {
            this.dispatcher.Initialize();
        }
    }
}
