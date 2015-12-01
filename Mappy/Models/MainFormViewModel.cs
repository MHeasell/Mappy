namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.Data;
    using Mappy.Database;

    public class MainFormViewModel : IMainFormModel
    {
        public MainFormViewModel(CoreModel model)
        {
            this.CanUndo = model.PropertyAsObservable(x => x.CanUndo, "CanUndo");
            this.CanRedo = model.PropertyAsObservable(x => x.CanRedo, "CanRedo");
            this.CanCut = model.PropertyAsObservable(x => x.CanCut, "CanCut");
            this.CanCopy = model.PropertyAsObservable(x => x.CanCopy, "CanCopy");
            this.CanPaste = model.PropertyAsObservable(x => x.CanPaste, "CanPaste");
            this.IsDirty = model.PropertyAsObservable(x => x.IsDirty, "IsDirty");
            this.MapOpen = model.PropertyAsObservable(x => x.MapOpen, "MapOpen");
            this.FilePath = model.PropertyAsObservable(x => x.FilePath, "FilePath");
            this.IsFileReadOnly = model.PropertyAsObservable(x => x.IsFileReadOnly, "IsFileReadOnly");
            this.GridVisible = model.PropertyAsObservable(x => x.GridVisible, "GridVisible");
            this.GridSize = model.PropertyAsObservable(x => x.GridSize, "GridSize");
            this.HeightmapVisible = model.PropertyAsObservable(x => x.HeightmapVisible, "HeightmapVisible");
            this.FeaturesVisible = model.PropertyAsObservable(x => x.FeaturesVisible, "FeaturesVisible");
            this.MinimapVisible = model.PropertyAsObservable(x => x.MinimapVisible, "MinimapVisible");
            this.SeaLevel = model.PropertyAsObservable(x => x.SeaLevel, "SeaLevel");

            this.FeatureRecords = model.PropertyAsObservable(x => x.FeatureRecords, "FeatureRecords");
            this.Sections = model.PropertyAsObservable(x => x.Sections, "Sections");
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
