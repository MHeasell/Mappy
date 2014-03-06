namespace Mappy.Presentation
{
    using System.ComponentModel;

    using Mappy.Models;

    public interface IMapDataModel : INotifyPropertyChanged
    {
        IBindingMapModel Map { get; }
    }
}