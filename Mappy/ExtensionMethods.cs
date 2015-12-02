namespace Mappy
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public static class ExtensionMethods
    {
        public static IObservable<TField> PropertyAsObservable<TSource, TField>(this TSource source, Func<TSource, TField> accessor, string name) where TSource : INotifyPropertyChanged
        {
            var obs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => source.PropertyChanged += x,
                x => source.PropertyChanged -= x)
                .Where(x => x.EventArgs.PropertyName == name)
                .Select(_ => accessor(source))
                .Multicast(new BehaviorSubject<TField>(accessor(source)));

            // FIXME: This is leaky.
            // We create a connection here but don't hold on to the reference.
            // We can never unregister our event handler without this.
            obs.Connect();

            return obs;
        }
    }
}
