namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Mappy.Data;
    using Mappy.Database;

    public class MainFormViewModel : IMainFormModel
    {
        private const string ProgramName = "Mappy";

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
    }
}
