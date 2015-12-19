namespace Mappy.Models
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Database;

    public interface IMapViewSettingsModel
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

        void SetViewportSize(Size size);

        void SetViewportLocation(Point pos);

        void SetViewportRectangle(Rectangle viewport);

        void OpenFromDragDrop(string filename);

        void DragDropData(IDataObject data, Point loc);

        void DeleteSelection();

        void ClearSelection();
    }
}
