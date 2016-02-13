namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using System.Reactive.Linq;
    using System.Windows.Forms;

    using Mappy.Data;
    using Mappy.Database;
    using Mappy.UI.Controls;
    using Mappy.Util;

    public class MapViewViewModel : Notifier, IMapViewSettingsModel
    {
        private readonly CoreModel model;

        private int? selectedTile;

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

            this.Map
                .Select(x => x?.PropertyAsObservable(y => y.SelectedTile, "SelectedTile") ?? Observable.Return<int?>(null))
                .Switch()
                .Subscribe(x => this.SelectedTile = x);

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

        public int? SelectedTile
        {
            get
            {
                return this.selectedTile;
            }

            set
            {
                this.SetField(ref this.selectedTile, value, nameof(this.SelectedTile));
            }
        }

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
                return;
            }

            if (data.GetDataPresent(DataFormats.Text))
            {
                var dataString = (string)data.GetData(DataFormats.Text);
                int id;
                if (int.TryParse(dataString, out id))
                {
                    this.model.DragDropSection(id, loc.X, loc.Y);
                }
                else
                {
                    this.model.DragDropFeature(dataString, loc.X, loc.Y);
                }

                return;
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

        public void DragDropStartPosition(int index, int x, int y)
        {
            this.model.DragDropStartPosition(index, x, y);
        }

        public void DragDropTile(IMapTile tile, int x, int y)
        {
            this.model.DragDropTile(tile, x, y);
        }

        public void DragDropFeature(string name, int x, int y)
        {
            this.model.DragDropFeature(name, x, y);
        }

        public void StartBandbox(int x, int y)
        {
            this.model.StartBandbox(x, y);
        }

        public void GrowBandbox(int x, int y)
        {
            this.model.GrowBandbox(x, y);
        }

        public void CommitBandbox()
        {
            this.model.CommitBandbox();
        }

        public void TranslateSelection(int x, int y)
        {
            this.model.TranslateSelection(x, y);
        }

        public void FlushTranslation()
        {
            this.model.FlushTranslation();
        }

        public void SelectTile(int index)
        {
            this.model.SelectTile(index);
        }

        public void SelectFeature(Guid id)
        {
            this.model.SelectFeature(id);
        }

        public void SelectStartPosition(int index)
        {
            this.model.SelectStartPosition(index);
        }
    }
}
