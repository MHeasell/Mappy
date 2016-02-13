namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class MinimapFormViewModel : IMinimapFormViewModel
    {
        private readonly CoreModel model;

        private readonly BehaviorSubject<Bitmap> minimapImage;

        private readonly BehaviorSubject<int> mapWidth;

        private readonly BehaviorSubject<int> mapHeight;

        private readonly BehaviorSubject<Rectangle> minimapRect;

        private bool mouseDown;

        public MinimapFormViewModel(CoreModel model)
        {
            var viewportLocation = model.PropertyAsObservable(x => x.ViewportLocation, "ViewportLocation");
            var viewportWidth = model.PropertyAsObservable(x => x.ViewportWidth, "ViewportWidth");
            var viewportHeight = model.PropertyAsObservable(x => x.ViewportHeight, "ViewportHeight");

            this.mapWidth = model.PropertyAsObservable(x => x.MapWidth, "MapWidth");
            this.mapHeight = model.PropertyAsObservable(x => x.MapHeight, "MapHeight");
            this.MinimapVisible = model.PropertyAsObservable(x => x.MinimapVisible, "MinimapVisible");

            this.minimapImage = model.PropertyAsObservable(x => x.MinimapImage, "MinimapImage");

            // set up the minimap rectangle observable
            var minimapRectWidth = this.ScaleObsWidthToMinimap(viewportWidth);
            var minimapRectHeight = this.ScaleObsHeightToMinimap(viewportHeight);
            var minimapRectSize = minimapRectWidth.CombineLatest(minimapRectHeight, (w, h) => new Size(w, h));

            var minimapRectX = this.ScaleObsWidthToMinimap(viewportLocation.Select(x => x.X));
            var minimapRectY = this.ScaleObsHeightToMinimap(viewportLocation.Select(x => x.Y));
            var minimapRectLocation = minimapRectX.CombineLatest(minimapRectY, (x, y) => new Point(x, y));

            this.minimapRect = new BehaviorSubject<Rectangle>(Rectangle.Empty);
            minimapRectLocation
                .CombineLatest(minimapRectSize, (l, s) => new Rectangle(l, s))
                .Subscribe(this.minimapRect);

            this.model = model;
        }

        public IObservable<bool> MinimapVisible { get; }

        public IObservable<Bitmap> MinimapImage => this.minimapImage;

        public IObservable<Rectangle> MinimapRect => this.minimapRect;

        public void MouseDown(Point location)
        {
            this.mouseDown = true;
            this.SetModelViewportCenter(location);
        }

        public void MouseMove(Point location)
        {
            if (this.mouseDown)
            {
                this.SetModelViewportCenter(location);
            }
        }

        public void MouseUp()
        {
            this.mouseDown = false;
        }

        public void FormCloseButtonClick()
        {
            this.model.HideMinimap();
        }

        private IObservable<int> ScaleObsWidthToMinimap(IObservable<int> value)
        {
            var mapWidth = this.mapWidth.Select(x => (x * 32) - 32);
            var minimapWidth = this.MinimapImage.Select(x => x?.Width ?? 0);

            return value
                .CombineLatest(minimapWidth, (v, w) => v * w)
                .CombineLatest(mapWidth, (v, w) => v / w);
        }

        private IObservable<int> ScaleObsHeightToMinimap(IObservable<int> value)
        {
            var mapHeight = this.mapHeight.Select(x => (x * 32) - 128);
            var minimapHeight = this.MinimapImage.Select(x => x?.Height ?? 0);

            return value
                .CombineLatest(minimapHeight, (v, h) => v * h)
                .CombineLatest(mapHeight, (v, h) => v / h);
        }

        private void SetModelViewportCenter(Point location)
        {
            var image = this.minimapImage.Value;
            if (image == null)
            {
                return;
            }

            var rect = this.minimapRect.Value;

            int x = location.X - (rect.Width / 2);
            int y = location.Y - (rect.Height / 2);

            x = this.ScaleWidthToMap(x);
            y = this.ScaleHeightToMap(y);

            this.model.SetViewportLocation(new Point(x, y));
        }

        private int ScaleWidthToMap(int val)
        {
            int mapWidth = (this.mapWidth.Value * 32) - 32;
            int minimapWidth = this.minimapImage.Value.Width;
            return (val * mapWidth) / minimapWidth;
        }

        private int ScaleHeightToMap(int val)
        {
            int mapHeight = (this.mapHeight.Value * 32) - 128;
            int minimapHeight = this.minimapImage.Value.Height;
            return (val * mapHeight) / minimapHeight;
        }
    }
}
