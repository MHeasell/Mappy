namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Mappy.Data;
    using Mappy.Database;

    public class MainFormViewModel : IMainFormModel
    {
        private readonly CoreModel model;

        public MainFormViewModel(CoreModel model)
        {
            this.model = model;

            var modelPropertyChanged =
                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    x => this.model.PropertyChanged += x,
                    x => this.model.PropertyChanged -= x)
                    .Select(x => x.EventArgs.PropertyName);

            var canUndo = new BehaviorSubject<bool>(this.model.CanUndo);
            modelPropertyChanged
                .Where(x => x == "CanUndo")
                .Select(_ => this.model.CanUndo)
                .Subscribe(canUndo);
            this.CanUndo = canUndo;

            var canRedo = new BehaviorSubject<bool>(this.model.CanRedo);
            modelPropertyChanged
                .Where(x => x == "CanRedo")
                .Select(_ => this.model.CanRedo)
                .Subscribe(canRedo);
            this.CanRedo = canRedo;

            var canCut = new BehaviorSubject<bool>(this.model.CanCut);
            modelPropertyChanged
                .Where(x => x == "CanCut")
                .Select(_ => this.model.CanCut)
                .Subscribe(canCut);
            this.CanCut = canCut;

            var canCopy = new BehaviorSubject<bool>(this.model.CanCopy);
            modelPropertyChanged
                .Where(x => x == "CanCopy")
                .Select(_ => this.model.CanCopy)
                .Subscribe(canCopy);
            this.CanCopy = canCopy;

            var canPaste = new BehaviorSubject<bool>(this.model.CanPaste);
            modelPropertyChanged
                .Where(x => x == "CanPaste")
                .Select(_ => this.model.CanPaste)
                .Subscribe(canPaste);
            this.CanPaste = canPaste;

            var isDirty = new BehaviorSubject<bool>(this.model.IsDirty);
            modelPropertyChanged
                .Where(x => x == "IsDirty")
                .Select(_ => this.model.IsDirty)
                .Subscribe(isDirty);
            this.IsDirty = isDirty;

            var mapOpen = new BehaviorSubject<bool>(this.model.MapOpen);
            modelPropertyChanged
                .Where(x => x == "MapOpen")
                .Select(_ => this.model.MapOpen)
                .Subscribe(mapOpen);
            this.MapOpen = mapOpen;

            var filePath = new BehaviorSubject<string>(this.model.FilePath);
            modelPropertyChanged
                .Where(x => x == "FilePath")
                .Select(_ => this.model.FilePath)
                .Subscribe(filePath);
            this.FilePath = filePath;

            var isFileReadOnly = new BehaviorSubject<bool>(this.model.IsFileReadOnly);
            modelPropertyChanged
                .Where(x => x == "IsFileReadOnly")
                .Select(_ => this.model.IsFileReadOnly)
                .Subscribe(isFileReadOnly);
            this.IsFileReadOnly = isFileReadOnly;

            var gridVisible = new BehaviorSubject<bool>(this.model.GridVisible);
            modelPropertyChanged
                .Where(x => x == "GridVisible")
                .Select(_ => this.model.GridVisible)
                .Subscribe(gridVisible);
            this.GridVisible = gridVisible;

            var gridSize = new BehaviorSubject<Size>(this.model.GridSize);
            modelPropertyChanged
                .Where(x => x == "GridSize")
                .Select(_ => this.model.GridSize)
                .Subscribe(gridSize);
            this.GridSize = gridSize;

            var heightmapVisible = new BehaviorSubject<bool>(this.model.HeightmapVisible);
            modelPropertyChanged
                .Where(x => x == "HeightmapVisible")
                .Select(_ => this.model.HeightmapVisible)
                .Subscribe(heightmapVisible);
            this.HeightmapVisible = heightmapVisible;

            var featuresVisible = new BehaviorSubject<bool>(this.model.FeaturesVisible);
            modelPropertyChanged
                .Where(x => x == "FeaturesVisible")
                .Select(_ => this.model.FeaturesVisible)
                .Subscribe(featuresVisible);
            this.FeaturesVisible = featuresVisible;

            var minimapVisible = new BehaviorSubject<bool>(this.model.MinimapVisible);
            modelPropertyChanged
                .Where(x => x == "MinimapVisible")
                .Select(_ => this.model.MinimapVisible)
                .Subscribe(minimapVisible);
            this.MinimapVisible = minimapVisible;

            var seaLevel = new BehaviorSubject<int>(this.model.SeaLevel);
            modelPropertyChanged
                .Where(x => x == "SeaLevel")
                .Select(_ => this.model.SeaLevel)
                .Subscribe(seaLevel);
            this.SeaLevel = seaLevel;

            var featureRecords = new BehaviorSubject<IFeatureDatabase>(this.model.FeatureRecords);
            modelPropertyChanged
                .Where(x => x == "FeatureRecords")
                .Select(_ => this.model.FeatureRecords)
                .Subscribe(featureRecords);
            this.FeatureRecords = featureRecords;

            var sections = new BehaviorSubject<IList<Section>>(this.model.Sections);
            modelPropertyChanged
                .Where(x => x == "Sections")
                .Select(_ => this.model.Sections)
                .Subscribe(sections);
            this.Sections = sections;
        }

        public IObservable<IFeatureDatabase> FeatureRecords { get; }

        public IObservable<IList<Section>> Sections { get; }

        public IObservable<bool> CanUndo { get; }

        public IObservable<bool> CanRedo { get; }

        public IObservable<bool> CanCut { get; }

        public IObservable<bool> CanCopy { get; }

        public IObservable<bool> CanPaste { get; }

        public IObservable<bool> IsDirty { get; }

        public IObservable<bool> MapOpen { get; }

        public IObservable<string> FilePath { get; }

        public IObservable<bool> IsFileReadOnly { get; }

        public IObservable<bool> GridVisible { get; }

        public IObservable<Size> GridSize { get; }

        public IObservable<bool> HeightmapVisible { get; }

        public IObservable<bool> FeaturesVisible { get; }

        public IObservable<bool> MinimapVisible { get; }

        public IObservable<int> SeaLevel { get; }
    }
}
