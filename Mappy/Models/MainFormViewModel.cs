namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reactive.Linq;

    using Mappy.Data;
    using Mappy.Database;

    public class MainFormViewModel : IMainFormViewModel
    {
        private const string ProgramName = "Mappy";

        private readonly CoreModel model;

        public MainFormViewModel(CoreModel model)
        {
            var mapOpen = model.PropertyAsObservable(x => x.MapOpen, "MapOpen");
            var isDirty = model.PropertyAsObservable(x => x.IsDirty, "IsDirty");
            var filePath = model.PropertyAsObservable(x => x.FilePath, "FilePath");
            var isFileReadOnly = model.PropertyAsObservable(x => x.IsFileReadOnly, "IsFileReadOnly");

            this.CanUndo = model.PropertyAsObservable(x => x.CanUndo, "CanUndo");
            this.CanRedo = model.PropertyAsObservable(x => x.CanRedo, "CanRedo");
            this.CanCut = model.PropertyAsObservable(x => x.CanCut, "CanCut");
            this.CanCopy = model.PropertyAsObservable(x => x.CanCopy, "CanCopy");
            this.CanPaste = model.PropertyAsObservable(x => x.CanPaste, "CanPaste");
            this.GridVisible = model.PropertyAsObservable(x => x.GridVisible, "GridVisible");
            this.GridSize = model.PropertyAsObservable(x => x.GridSize, "GridSize");
            this.HeightmapVisible = model.PropertyAsObservable(x => x.HeightmapVisible, "HeightmapVisible");
            this.FeaturesVisible = model.PropertyAsObservable(x => x.FeaturesVisible, "FeaturesVisible");
            this.MinimapVisible = model.PropertyAsObservable(x => x.MinimapVisible, "MinimapVisible");
            this.SeaLevel = model.PropertyAsObservable(x => x.SeaLevel, "SeaLevel");

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

            // TODO: Come up with some other solution for this.
            // It should be possible to observe add/remove events from these collections
            // rather than listening to the collections themselves.
            // Needs some refactoring in CoreModel.
            this.FeatureRecords = model.PropertyAsObservable(x => x.FeatureRecords, "FeatureRecords");
            this.Sections = model.PropertyAsObservable(x => x.Sections, "Sections");

            // set up CanSave observable
            var canSave = Observable.CombineLatest(
                mapOpen,
                filePath.Select(x => x != null),
                isFileReadOnly.Select(x => !x))
                .Select(x => x.All(y => y))
                .Replay(1);
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

            this.model = model;
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

        public IObservable<IFeatureDatabase> FeatureRecords { get; }

        public IObservable<IList<Section>> Sections { get; }

        public IObservable<bool> CanUndo { get; }

        public IObservable<bool> CanRedo { get; }

        public IObservable<bool> CanCut { get; }

        public IObservable<bool> CanCopy { get; }

        public IObservable<bool> CanPaste { get; }

        public IObservable<bool> GridVisible { get; }

        public IObservable<Size> GridSize { get; }

        public IObservable<bool> HeightmapVisible { get; }

        public IObservable<bool> FeaturesVisible { get; }

        public IObservable<bool> MinimapVisible { get; }

        public IObservable<int> SeaLevel { get; }

        public void ToggleHeightmap()
        {
            this.model.ToggleHeightmap();
        }

        public void ToggleMinimap()
        {
            this.model.ToggleMinimap();
        }

        public void ToggleFeatures()
        {
            this.model.ToggleFeatures();
        }

        public void OpenPreferences()
        {
            this.model.OpenPreferences();
        }

        public void ShowAbout()
        {
            this.model.ShowAbout();
        }

        public void OpenMapAttributes()
        {
            this.model.OpenMapAttributes();
        }

        public void ChooseColor()
        {
            this.model.ChooseColor();
        }

        public bool New()
        {
            return this.model.New();
        }

        public bool Open()
        {
            return this.model.Open();
        }

        public bool OpenFromDragDrop(string filename)
        {
            return this.model.OpenFromDragDrop(filename);
        }

        public bool Save()
        {
            return this.model.Save();
        }

        public bool SaveAs()
        {
            return this.model.SaveAs();
        }

        public void CloseMap()
        {
            this.model.CloseMap();
        }

        public void Undo()
        {
            this.model.Undo();
        }

        public void Redo()
        {
            this.model.Redo();
        }

        public void Close()
        {
            this.model.Close();
        }

        public void RefreshMinimap()
        {
            this.model.RefreshMinimap();
        }

        public void RefreshMinimapHighQualityWithProgress()
        {
            this.model.RefreshMinimapHighQualityWithProgress();
        }

        public void HideGrid()
        {
            this.model.HideGrid();
        }

        public void EnableGridWithSize(Size s)
        {
            this.model.EnableGridWithSize(s);
        }

        public void SetSeaLevel(int value)
        {
            this.model.SetSeaLevel(value);
        }

        public void FlushSeaLevel()
        {
            this.model.FlushSeaLevel();
        }

        public void CopySelectionToClipboard()
        {
            this.model.CopySelectionToClipboard();
        }

        public void CutSelectionToClipboard()
        {
            this.model.CutSelectionToClipboard();
        }

        public void PasteFromClipboard()
        {
            this.model.PasteFromClipboard();
        }

        public void ImportMinimap()
        {
            this.model.ImportMinimap();
        }

        public void ExportMinimap()
        {
            this.model.ExportMinimap();
        }

        public void ImportHeightmap()
        {
            this.model.ImportHeightmap();
        }

        public void ExportHeightmap()
        {
            this.model.ExportHeightmap();
        }

        public void ExportMapImage()
        {
            this.model.ExportMapImage();
        }

        public void ImportCustomSection()
        {
            this.model.ImportCustomSection();
        }

        public void Initialize()
        {
            this.model.Initialize();
        }
    }
}
