namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Data;
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

        IObservable<int> ViewportWidth { get; }

        IObservable<int> ViewportHeight { get; }

        void SetViewportSize(Size size);

        void SetViewportLocation(Point pos);

        void OpenFromDragDrop(string filename);

        void DragDropData(IDataObject data, Point loc);
    }
}
