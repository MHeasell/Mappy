namespace Mappy.Presentation
{
    using System.ComponentModel;

    using Mappy.Models;

    public interface IMapPresenterModel : INotifyPropertyChanged
    {
        IBindingMapModel Map { get; }
    }
}