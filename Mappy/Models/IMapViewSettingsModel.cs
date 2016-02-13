namespace Mappy.Models
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Data;
    using Mappy.Database;

    public interface IMapViewSettingsModel : INotifyPropertyChanged
    {
        IObservable<bool> GridVisible { get; }

        IObservable<Color> GridColor { get; }

        IObservable<Size> GridSize { get; }

        IObservable<bool> HeightmapVisible { get; }

        IObservable<bool> FeaturesVisible { get; }

        IObservable<IFeatureDatabase> FeatureRecords { get; }

        IObservable<IMainModel> Map { get; }

        IObservable<Point> ViewportLocation { get; }

        IObservable<Size> CanvasSize { get; }

        int? SelectedTile { get; }

        void SetViewportSize(Size size);

        void SetViewportLocation(Point pos);

        void SetViewportRectangle(Rectangle viewport);

        void OpenFromDragDrop(string filename);

        void DragDropData(IDataObject data, Point loc);

        void DeleteSelection();

        void ClearSelection();

        void DragDropStartPosition(int index, int x, int y);

        void DragDropTile(IMapTile tile, int x, int y);

        void DragDropFeature(string name, int x, int y);

        void StartBandbox(int x, int y);

        void GrowBandbox(int x, int y);

        void CommitBandbox();

        void TranslateSelection(int x, int y);

        void FlushTranslation();

        void SelectTile(int index);

        void SelectFeature(Guid id);

        void SelectStartPosition(int index);
    }
}
