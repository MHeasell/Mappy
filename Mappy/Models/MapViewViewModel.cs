namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.Data;
    using Mappy.Database;

    public class MapViewViewModel : IMapViewSettingsModel
    {
        private readonly CoreModel model;

        public MapViewViewModel(CoreModel model)
        {
            this.GridVisible = model.PropertyAsObservable(x => x.GridVisible, "GridVisible");
            this.GridColor = model.PropertyAsObservable(x => x.GridColor, "GridColor");
            this.GridSize = model.PropertyAsObservable(x => x.GridSize, "GridSize");
            this.HeightmapVisible = model.PropertyAsObservable(x => x.HeightmapVisible, "HeightmapVisible");
            this.FeaturesVisible = model.PropertyAsObservable(x => x.FeaturesVisible, "FeaturesVisible");
            this.Sections = model.PropertyAsObservable(x => x.Sections, "Sections");
            this.FeatureRecords = model.PropertyAsObservable(x => x.FeatureRecords, "FeatureRecords");
            this.Map = model.PropertyAsObservable(x => x.Map, "Map");
            this.ViewportWidth = model.PropertyAsObservable(x => x.ViewportWidth, "ViewportWidth");
            this.ViewportHeight = model.PropertyAsObservable(x => x.ViewportHeight, "ViewportHeight");

            this.model = model;
        }

        public IObservable<bool> GridVisible { get; }

        public IObservable<Color> GridColor { get; }

        public IObservable<Size> GridSize { get; }

        public IObservable<bool> HeightmapVisible { get; }

        public IObservable<bool> FeaturesVisible { get; }

        public IObservable<IList<Section>> Sections { get; }

        public IObservable<IFeatureDatabase> FeatureRecords { get; }

        public IObservable<IMainModel> Map { get; }

        public IObservable<int> ViewportWidth { get; set; }

        public IObservable<int> ViewportHeight { get; set; }

        public void SetViewportSize(Size size)
        {
            this.model.SetViewportSize(size);
        }

        public void SetViewportLocation(Point pos)
        {
            this.model.SetViewportLocation(pos);
        }

        public void OpenFromDragDrop(string filename)
        {
            this.model.OpenFromDragDrop(filename);
        }
    }
}
