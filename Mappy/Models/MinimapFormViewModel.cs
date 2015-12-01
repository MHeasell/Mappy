namespace Mappy.Models
{
    using System;
    using System.Drawing;

    public class MinimapFormViewModel : IMinimapModel
    {
        private CoreModel model;

        public MinimapFormViewModel(CoreModel model)
        {
            this.model = model;

            this.MapWidth = model.PropertyAsObservable(x => x.MapWidth, "MapWidth");
            this.MapHeight = model.PropertyAsObservable(x => x.MapHeight, "MapHeight");
            this.MinimapVisible = model.PropertyAsObservable(x => x.MinimapVisible, "MinimapVisible");
            this.ViewportLocation = model.PropertyAsObservable(x => x.ViewportLocation, "ViewportLocation");
            this.ViewportWidth = model.PropertyAsObservable(x => x.ViewportWidth, "ViewportWidth");
            this.ViewportHeight = model.PropertyAsObservable(x => x.ViewportHeight, "ViewportHeight");
            this.MinimapImage = model.PropertyAsObservable(x => x.MinimapImage, "MinimapImage");
        }

        public IObservable<int> MapWidth { get; }

        public IObservable<int> MapHeight { get; }

        public IObservable<bool> MinimapVisible { get; }

        public IObservable<Point> ViewportLocation { get; }

        public IObservable<int> ViewportWidth { get; }

        public IObservable<int> ViewportHeight { get; }

        public IObservable<Bitmap> MinimapImage { get; }
    }
}
