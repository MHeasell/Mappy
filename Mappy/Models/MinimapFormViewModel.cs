namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class MinimapFormViewModel : IMinimapModel
    {
        public MinimapFormViewModel(CoreModel model)
        {
            var viewportLocation = model.PropertyAsObservable(x => x.ViewportLocation, "ViewportLocation");
            var viewportWidth = model.PropertyAsObservable(x => x.ViewportWidth, "ViewportWidth");
            var viewportHeight = model.PropertyAsObservable(x => x.ViewportHeight, "ViewportHeight");

            this.MapWidth = model.PropertyAsObservable(x => x.MapWidth, "MapWidth");
            this.MapHeight = model.PropertyAsObservable(x => x.MapHeight, "MapHeight");
            this.MinimapVisible = model.PropertyAsObservable(x => x.MinimapVisible, "MinimapVisible");
            this.MinimapImage = model.PropertyAsObservable(x => x.MinimapImage, "MinimapImage");

            // set up the minimap rectangle observable
            var minimapRect = new BehaviorSubject<Rectangle>(Rectangle.Empty);

            var minimapRectWidth = this.ScaleObsWidthToMinimap(viewportWidth);
            var minimapRectHeight = this.ScaleObsHeightToMinimap(viewportHeight);
            var minimapRectSize = minimapRectWidth.CombineLatest(minimapRectHeight, (w, h) => new Size(w, h));

            var minimapRectX = this.ScaleObsWidthToMinimap(viewportLocation.Select(x => x.X));
            var minimapRectY = this.ScaleObsHeightToMinimap(viewportLocation.Select(x => x.Y));
            var minimapRectLocation = minimapRectX.CombineLatest(minimapRectY, (x, y) => new Point(x, y));

            minimapRectLocation
                .CombineLatest(minimapRectSize, (l, s) => new Rectangle(l, s))
                .Subscribe(minimapRect);

            this.MinimapRect = minimapRect;
        }

        public IObservable<int> MapWidth { get; }

        public IObservable<int> MapHeight { get; }

        public IObservable<bool> MinimapVisible { get; }

        public IObservable<Bitmap> MinimapImage { get; }

        public IObservable<Rectangle> MinimapRect { get; }

        private IObservable<int> ScaleObsWidthToMinimap(IObservable<int> value)
        {
            var mapWidth = this.MapWidth.Select(x => (x * 32) - 32);
            var minimapWidth = this.MinimapImage.Select(x => x?.Width ?? 0);

            return value
                .CombineLatest(minimapWidth, (v, w) => v * w)
                .CombineLatest(mapWidth, (v, w) => v / w);
        }

        private IObservable<int> ScaleObsHeightToMinimap(IObservable<int> value)
        {
            var mapHeight = this.MapHeight.Select(x => (x * 32) - 128);
            var minimapHeight = this.MinimapImage.Select(x => x?.Height ?? 0);

            return value
                .CombineLatest(minimapHeight, (v, h) => v * h)
                .CombineLatest(mapHeight, (v, h) => v / h);
        }
    }
}
