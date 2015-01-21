namespace Mappy.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Data;
    using Mappy.Database;

    public interface IMapViewSettingsModel : INotifyPropertyChanged
    {
        bool GridVisible { get; }

        Color GridColor { get; }

        Size GridSize { get; }

        bool HeightmapVisible { get; }

        bool FeaturesVisible { get; }

        IList<Section> Sections { get; }

        IFeatureDatabase FeatureRecords { get; }

        IMainModel Map { get; }

        int ViewportWidth { get; set; }

        int ViewportHeight { get; set; }
    }
}
