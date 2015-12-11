namespace Mappy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Mappy.Data;
    using Mappy.Database;

    public interface IMapViewSettingsModel
    {
        IObservable<bool> GridVisible { get; }

        IObservable<Color> GridColor { get; }

        IObservable<Size> GridSize { get; }

        IObservable<bool> HeightmapVisible { get; }

        IObservable<bool> FeaturesVisible { get; }

        IObservable<IList<Section>> Sections { get; }

        IObservable<IFeatureDatabase> FeatureRecords { get; }

        IObservable<IMainModel> Map { get; }

        IObservable<int> ViewportWidth { get; set; }

        IObservable<int> ViewportHeight { get; set; }

        void SetViewportSize(Size size);

        void SetViewportLocation(Point pos);

        void OpenFromDragDrop(string filename);
    }
}
