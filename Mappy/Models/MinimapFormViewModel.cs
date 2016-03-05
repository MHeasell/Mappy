namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using System.Reactive;
    using System.Reactive.Linq;

    using Mappy.Services;
    using Mappy.Util;

    public sealed class MinimapFormViewModel : Notifier, IMinimapFormViewModel
    {
        private static readonly Color[] StartPositionColors = new[]
            {
                Color.FromArgb(0, 0, 255),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(0, 255, 0),
                Color.FromArgb(0, 0, 128),
                Color.FromArgb(128, 0, 255),
                Color.FromArgb(255, 255, 0),
                Color.FromArgb(0, 0, 0),
                Color.FromArgb(128, 128, 255),
                Color.FromArgb(255, 180, 140),
            };

        private readonly IReadOnlyApplicationModel model;

        private readonly Dispatcher dispatcher;

        private bool minimapVisible;

        private Maybe<Bitmap> minimapImage;

        private Rectangle minimapRect;

        private bool mouseDown;

        public MinimapFormViewModel(IReadOnlyApplicationModel model, Dispatcher dispatcher)
        {
            this.model = model;
            this.dispatcher = dispatcher;

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

            // wire up the simple properties
            minimapVisible.Subscribe(x => this.MinimapVisible = x);
            minimap.Subscribe(x => this.MinimapImage = Maybe.From(x));

            // listen for changes that affect the minimap viewport rectangle
            mapChanged.Subscribe(_ => this.UpdateMinimapRectangle());
            minimapChanged.Subscribe(_ => this.UpdateMinimapRectangle());
            viewportLocationChanged.Subscribe(_ => this.UpdateMinimapRectangle());
            viewportWidthChanged.Subscribe(_ => this.UpdateMinimapRectangle());
            viewportHeightChanged.Subscribe(_ => this.UpdateMinimapRectangle());

            // do the initial rect update
            this.UpdateMinimapRectangle();
        }

        public bool MinimapVisible
        {
            get
            {
                return this.minimapVisible;
            }

            private set
            {
                this.SetField(ref this.minimapVisible, value, nameof(this.MinimapVisible));
            }
        }

        public Maybe<Bitmap> MinimapImage
        {
            get
            {
                return this.minimapImage;
            }

            private set
            {
                this.SetField(ref this.minimapImage, value, nameof(this.MinimapImage));
            }
        }

        public Rectangle MinimapRect
        {
            get
            {
                return this.minimapRect;
            }

            private set
            {
                this.SetField(ref this.minimapRect, value, nameof(this.MinimapRect));
            }
        }

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
            Console.WriteLine("updating minimap rect");

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
    }
}
