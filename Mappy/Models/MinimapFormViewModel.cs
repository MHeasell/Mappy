namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class MinimapFormViewModel : IMinimapFormViewModel
    {
        private readonly CoreModel model;

        // subjects from user events
        private readonly ISubject<Point> mousePosition = new Subject<Point>();

        private readonly ISubject<bool> mouseDown = new BehaviorSubject<bool>(false);

        public MinimapFormViewModel(CoreModel model)
        {
            // set up observables from properties
            var viewportLocation = model.PropertyAsObservable(x => x.ViewportLocation, "ViewportLocation");
            var viewportWidth = model.PropertyAsObservable(x => x.ViewportWidth, "ViewportWidth");
            var viewportHeight = model.PropertyAsObservable(x => x.ViewportHeight, "ViewportHeight");

            var map = model.PropertyAsObservable(x => x.Map, nameof(model.Map));

            var mapWidth = map.ObservePropertyOrDefault(x => x.MapWidth, "MapWidth", 0);
            var mapHeight = map.ObservePropertyOrDefault(x => x.MapHeight, "MapHeight", 0);

            this.MinimapVisible = model.PropertyAsObservable(x => x.MinimapVisible, "MinimapVisible");

            this.MinimapImage = map.ObservePropertyOrDefault(x => x.Minimap, "Minimap", null);

            // set up some computed observables
            var viewportSize = viewportWidth.CombineLatest(viewportHeight, (w, h) => new Size(w, h));
            var mapSize = mapWidth.CombineLatest(mapHeight, (w, h) => new Size(w, h));
            var mapVisiblePixelSize = mapSize.Select(s => new Size((s.Width * 32) - 32, (s.Height * 32) - 128));
            var minimapSize = this.MinimapImage.Select(x => x?.Size);

            // set up the minimap rectangle observable
            var minimapRectSize = ScaleToMinimap(viewportSize, mapVisiblePixelSize, minimapSize);
            var minimapRectLocation = ScaleToMinimap(viewportLocation, mapVisiblePixelSize, minimapSize);
            this.MinimapRect = minimapRectLocation
                .CombineLatest(minimapRectSize, (l, s) => new Rectangle(l, s));

            // wire up user events to the model
            var minimapRectExtents = minimapRectSize.Select(s => new Size(s.Width / 2, s.Height / 2));
            var minimapViewportTopLeft = this.mousePosition
                .CombineLatest(minimapRectExtents, (l, e) => new Point(l.X - e.Width, l.Y - e.Height));
            var newMapViewportLocation = ScaleToMap(minimapViewportTopLeft, mapVisiblePixelSize, minimapSize);

            newMapViewportLocation
                .Pausable(this.mouseDown)
                .Subscribe(model.SetViewportLocation);

            this.model = model;
        }

        public IObservable<bool> MinimapVisible { get; }

        public IObservable<Bitmap> MinimapImage { get; }

        public IObservable<Rectangle> MinimapRect { get; }

        public void MouseDown(Point location)
        {
            this.mouseDown.OnNext(true);
            this.mousePosition.OnNext(location);
        }

        public void MouseMove(Point location)
        {
            this.mousePosition.OnNext(location);
        }

        public void MouseUp()
        {
            this.mouseDown.OnNext(false);
        }

        public void FormCloseButtonClick()
        {
            this.model.HideMinimap();
        }

        private static IObservable<Point> ScaleToMinimap(
            IObservable<Point> value,
            IObservable<Size> mapWidth,
            IObservable<Size?> minimapWidth)
        {
            var newMinimapWidth = minimapWidth.Select(x => x ?? Size.Empty);

            return value
                .CombineLatest(newMinimapWidth, (v, w) => new Point(v.X * w.Width, v.Y * w.Height))
                .CombineLatest(mapWidth, (v, w) => new Point(v.X / w.Width, v.Y / w.Height));
        }

        private static IObservable<Size> ScaleToMinimap(
            IObservable<Size> value,
            IObservable<Size> mapWidth,
            IObservable<Size?> minimapWidth)
        {
            var newMinimapWidth = minimapWidth.Select(x => x ?? Size.Empty);

            return value
                .CombineLatest(newMinimapWidth, (v, w) => new Size(v.Width * w.Width, v.Height * w.Height))
                .CombineLatest(mapWidth, (v, w) => new Size(v.Width / w.Width, v.Height / w.Height));
        }

        private static IObservable<Point> ScaleToMap(
            IObservable<Point> value,
            IObservable<Size> mapSize,
            IObservable<Size?> minimapSize)
        {
            return value
                .CombineLatest(mapSize, (v, w) => new Point(v.X * w.Width, v.Y * w.Height))
                .CombineLatest(minimapSize, (v, w) => w.HasValue ? new Point(v.X / w.Value.Width, v.Y / w.Value.Height) : Point.Empty);
        }
    }
}
