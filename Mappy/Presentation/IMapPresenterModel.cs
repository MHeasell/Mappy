namespace Mappy.Presentation
{
    using System.ComponentModel;
    using System.Drawing;

    using Mappy.Models;

    public interface IMapPresenterModel : INotifyPropertyChanged
    {
        IBindingMapModel Map { get; }

        bool HeightmapVisible { get; }

        bool FeaturesVisible { get; }

        bool GridVisible { get; }

        Size GridSize { get; }

        Color GridColor { get; }
    }
}