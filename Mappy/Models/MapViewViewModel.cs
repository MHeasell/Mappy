namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using System.Reactive.Linq;
    using System.Windows.Forms;

    using Mappy.Database;
    using Mappy.UI.Controls;

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
            this.FeatureRecords = model.PropertyAsObservable(x => x.FeatureRecords, "FeatureRecords");
            this.Map = model.PropertyAsObservable(x => x.Map, "Map");

            this.ViewportLocation = model.PropertyAsObservable(x => x.ViewportLocation, "ViewportLocation");

            var mapWidth = model.PropertyAsObservable(x => x.MapWidth, "MapWidth");
            var mapHeight = model.PropertyAsObservable(x => x.MapHeight, "MapHeight");
            this.CanvasSize = mapWidth.CombineLatest(mapHeight, (w, h) => new Size(w * 32, h * 32));

            this.model = model;
        }

        public IObservable<bool> GridVisible { get; }

        public IObservable<Color> GridColor { get; }

        public IObservable<Size> GridSize { get; }

        public IObservable<bool> HeightmapVisible { get; }

        public IObservable<bool> FeaturesVisible { get; }

        public IObservable<IFeatureDatabase> FeatureRecords { get; }

        public IObservable<IMainModel> Map { get; }

        public IObservable<Point> ViewportLocation { get; }

        public IObservable<Size> CanvasSize { get; }

        public void SetViewportSize(Size size)
        {
            this.model.SetViewportSize(size);
        }

        public void SetViewportLocation(Point pos)
        {
            this.model.SetViewportLocation(pos);
        }

        public void SetViewportRectangle(Rectangle rect)
        {
            this.model.SetViewportLocation(rect.Location);
            this.model.SetViewportSize(rect.Size);
        }

        public void OpenFromDragDrop(string filename)
        {
            this.model.OpenFromDragDrop(filename);
        }

        public void DragDropData(IDataObject data, Point loc)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])data.GetData(DataFormats.FileDrop);
                if (files.Length < 1)
                {
                    return;
                }

                this.model.OpenFromDragDrop(files[0]);
                return;
            }

            if (data.GetDataPresent(typeof(StartPositionDragData)))
            {
                StartPositionDragData posData = (StartPositionDragData)data.GetData(typeof(StartPositionDragData));
                this.model.SetStartPosition(posData.PositionNumber, loc.X, loc.Y);
            }
            else
            {
                string dataString = data.GetData(DataFormats.Text).ToString();
                int id;
                if (int.TryParse(dataString, out id))
                {
                    this.model.DragDropSection(id, loc.X, loc.Y);
                }
                else
                {
                    this.model.DragDropFeature(dataString, loc.X, loc.Y);
                }
            }
        }

        public void DeleteSelection()
        {
            this.model.DeleteSelection();
        }

        public void ClearSelection()
        {
            this.model.ClearSelection();
        }
    }
}
