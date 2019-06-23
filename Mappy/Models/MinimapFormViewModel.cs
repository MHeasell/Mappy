namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Mappy.Data;
    using Mappy.Services;
    using Mappy.Util;

    public sealed class MinimapFormViewModel : Notifier, IMinimapFormViewModel
    {
        private readonly IReadOnlyApplicationModel model;

        private readonly Dispatcher dispatcher;

        private readonly IList<BehaviorSubject<Maybe<Point>>> startPositions;

        private bool minimapVisible;

        private Maybe<Bitmap> minimapImage;

        private Rectangle minimapRect;

        private bool mouseDown;

        public MinimapFormViewModel(IReadOnlyApplicationModel model, Dispatcher dispatcher)
        {
            this.model = model;
            this.dispatcher = dispatcher;
            this.startPositions = CreateStartPositionsArray();

            // set up basic properties as observables
            var minimapVisible = model.PropertyAsObservable(x => x.MinimapVisible, nameof(model.MinimapVisible));
            var map = model.PropertyAsObservable(x => x.Map, nameof(model.Map));
            var minimap = map.Select(x => x.Match(
                    y => y.PropertyAsObservable(z => z.Minimap, nameof(y.Minimap)),
                    () => Observable.Return<Bitmap>(null)))
                .Switch();

            // set up observables for property change notifications
            var mapChanged = model.PropertyChangedObservable(nameof(model.Map));
            var viewportWidthChanged = model.PropertyChangedObservable(nameof(model.ViewportWidth));
            var viewportHeightChanged = model.PropertyChangedObservable(nameof(model.ViewportHeight));
            var minimapChanged = map.Select(
                    x => x.Match(
                        y => y.PropertyChangedObservable(nameof(y.Minimap)),
                        Observable.Empty<Unit>))
                .Switch();
            var viewportLocationChanged = map.Select(
                    x => x.Match(
                        y => y.PropertyChangedObservable(nameof(y.ViewportLocation)),
                        Observable.Empty<Unit>))
                .Switch();

            var startPositionChanged = map.Select(
                x => x.Match(
                    y => Observable
                        .FromEventPattern<StartPositionChangedEventArgs>(
                            e => y.StartPositionChanged += e,
                            e => y.StartPositionChanged -= e)
                        .Select(z => z.EventArgs.Index),
                    Observable.Empty<int>))
                    .Switch();

            // wire up the simple properties
            minimapVisible.Subscribe(x => this.MinimapVisible = x);
            minimap.Subscribe(x => this.MinimapImage = Maybe.From(x));

            // listen for changes that affect the minimap viewport rectangle
            mapChanged.Subscribe(_ => this.UpdateMinimapRectangle());
            mapChanged.Subscribe(_ => this.UpdateStartPositions());

            minimapChanged.Subscribe(_ => this.UpdateMinimapRectangle());
            viewportLocationChanged.Subscribe(_ => this.UpdateMinimapRectangle());
            viewportWidthChanged.Subscribe(_ => this.UpdateMinimapRectangle());
            viewportHeightChanged.Subscribe(_ => this.UpdateMinimapRectangle());

            startPositionChanged.Subscribe(this.UpdateStartPosition);

            // do the initial rect update
            this.UpdateMinimapRectangle();
        }

        public bool MinimapVisible
        {
            get => this.minimapVisible;
            private set => this.SetField(ref this.minimapVisible, value, nameof(this.MinimapVisible));
        }

        public Maybe<Bitmap> MinimapImage
        {
            get => this.minimapImage;
            private set => this.SetField(ref this.minimapImage, value, nameof(this.MinimapImage));
        }

        public Rectangle MinimapRect
        {
            get => this.minimapRect;
            private set => this.SetField(ref this.minimapRect, value, nameof(this.MinimapRect));
        }

        public IList<BehaviorSubject<Maybe<Point>>> StartPositions => this.startPositions;

        public void Dispose()
        {
        }

        public void MouseDown(Point location)
        {
            this.mouseDown = true;
            this.UpdateMapViewportLocation(location);
        }

        public void MouseMove(Point location)
        {
            if (this.mouseDown)
            {
                this.UpdateMapViewportLocation(location);
            }
        }

        public void MouseUp()
        {
            this.mouseDown = false;
        }

        public void FormCloseButtonClick()
        {
            this.dispatcher.HideMinimap();
        }

        private static Size GetVisiblePixelSize(UndoableMapModel map)
        {
            var w = map.MapWidth;
            var h = map.MapHeight;
            return new Size((w * 32) - 32, (h * 32) - 128);
        }

        private static Point ScaleToMinimap(Point value, Size mapSize, Size minimapSize)
        {
            return new Point(
                (value.X * minimapSize.Width) / mapSize.Width,
                (value.Y * minimapSize.Height) / mapSize.Height);
        }

        private static Size ScaleToMinimap(Size value, Size mapSize, Size minimapSize)
        {
            return new Size(
                (value.Width * minimapSize.Width) / mapSize.Width,
                (value.Height * minimapSize.Height) / mapSize.Height);
        }

        private static Point ScaleToMap(Point value, Size mapSize, Size minimapSize)
        {
            return new Point(
                (value.X * mapSize.Width) / minimapSize.Width,
                (value.Y * mapSize.Height) / minimapSize.Height);
        }

        private static IList<BehaviorSubject<Maybe<Point>>> CreateStartPositionsArray()
        {
            var list = new List<BehaviorSubject<Maybe<Point>>>(10);
            for (var i = 0; i < 10; i++)
            {
                list.Add(new BehaviorSubject<Maybe<Point>>(Maybe.None<Point>()));
            }

            return list;
        }

        private void UpdateMapViewportLocation(Point minimapLocation)
        {
            if (this.model.Map.IsNone)
            {
                return;
            }

            var map = this.model.Map.UnsafeValue;

            if (this.MinimapImage.IsNone)
            {
                return;
            }

            var minimap = this.MinimapImage.UnsafeValue;

            var mapSize = GetVisiblePixelSize(map);
            var minimapSize = minimap.Size;
            var viewportSize = this.MinimapRect.Size;
            var minimapExtents = new Size(viewportSize.Width / 2, viewportSize.Height / 2);
            var minimapTopLeftLocation = minimapLocation - minimapExtents;
            var mapLocation = ScaleToMap(minimapTopLeftLocation, mapSize, minimapSize);

            this.dispatcher.SetViewportLocation(mapLocation);
        }

        private void UpdateMinimapRectangle()
        {
            if (this.model.Map.IsNone)
            {
                this.MinimapRect = Rectangle.Empty;
                return;
            }

            var map = this.model.Map.UnsafeValue;
            var minimap = map.Minimap;

            if (minimap == null)
            {
                this.MinimapRect = Rectangle.Empty;
                return;
            }

            var mapViewportLocation = map.ViewportLocation;
            var mapViewportSize = new Size(this.model.ViewportWidth, this.model.ViewportHeight);

            var mapSize = GetVisiblePixelSize(map);
            var minimapSize = minimap.Size;
            var minimapViewportLocation = ScaleToMinimap(mapViewportLocation, mapSize, minimapSize);
            var minimapViewportSize = ScaleToMinimap(mapViewportSize, mapSize, minimapSize);
            this.MinimapRect = new Rectangle(minimapViewportLocation, minimapViewportSize);
        }

        private void UpdateStartPositions()
        {
            if (this.model.Map.IsNone)
            {
                foreach (var t in this.startPositions)
                {
                    t.OnNext(Maybe.None<Point>());
                }

                return;
            }

            var map = this.model.Map.UnsafeValue;

            if (map.Minimap == null)
            {
                foreach (var t in this.startPositions)
                {
                    t.OnNext(Maybe.None<Point>());
                }

                return;
            }

            var minimap = map.Minimap;
            var mapSize = GetVisiblePixelSize(map);
            var minimapSize = minimap.Size;

            for (var i = 0; i < this.startPositions.Count; i++)
            {
                var pos = map
                    .GetStartPosition(i)
                    .ToMaybe()
                    .Map(x => ScaleToMinimap(x, mapSize, minimapSize));
                this.startPositions[i].OnNext(pos);
            }
        }

        private void UpdateStartPosition(int index)
        {
            // Map should never be null
            // since we're responding to an event from it.
            var map = this.model.Map.UnsafeValue;

            if (map.Minimap == null)
            {
                this.startPositions[index].OnNext(Maybe.None<Point>());
                return;
            }

            var minimap = map.Minimap;
            var mapSize = GetVisiblePixelSize(map);
            var minimapSize = minimap.Size;
            var newStartPosition = map
                .GetStartPosition(index)
                .ToMaybe()
                .Map(x => ScaleToMinimap(x, mapSize, minimapSize));
            this.startPositions[index].OnNext(newStartPosition);
        }
    }
}
