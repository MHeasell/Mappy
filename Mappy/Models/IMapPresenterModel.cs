namespace Mappy.Models
{
    using System.ComponentModel;
    using System.Drawing;

    public interface IMapPresenterModel : INotifyPropertyChanged
    {
        IBindingMapModel Map { get; }

        bool HeightmapVisible { get; set; }

        bool FeaturesVisible { get; set; }

        bool GridVisible { get; set; }

        Size GridSize { get; set; }

        Color GridColor { get; set; }
    }
}