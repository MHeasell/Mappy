namespace Mappy.Models
{
    using System.ComponentModel;

    public interface IMapDataModel : INotifyPropertyChanged
    {
        IBindingMapModel Map { get; }
    }
}