namespace Mappy.Models.Session
{
    using System.ComponentModel;
    using System.Drawing;

    public interface IViewOptionsModel : INotifyPropertyChanged
    {
        bool HeightmapVisible { get; }

        bool FeaturesVisible { get; }

        bool GridVisible { get; }

        Size GridSize { get; }

        Color GridColor { get; }
    }
}
