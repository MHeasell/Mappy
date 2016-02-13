namespace Mappy
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public static class ExtensionMethods
    {
        public static BehaviorSubject<TField> PropertyAsObservable<TSource, TField>(this TSource source, Func<TSource, TField> accessor, string name)
            where TSource : INotifyPropertyChanged
        {
            var subject = new BehaviorSubject<TField>(accessor(source));

            // FIXME: This is leaky.
            // We create a subscription here but don't hold on to the reference.
            // We can never unregister our event handler without this.
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => source.PropertyChanged += x,
                x => source.PropertyChanged -= x)
                .Where(x => x.EventArgs.PropertyName == name)
                .Select(_ => accessor(source))
                .Subscribe(subject);

            return subject;
        }

        /// <summary>
        /// Creates an observable that is paused while the pauser is false.
        /// Events from the source observable are dropped
        /// while in the paused state.
        /// </summary>
        public static IObservable<T> Pausable<T>(this IObservable<T> source, IObservable<bool> pauser)
        {
            var nothing = Observable.Empty<T>();
            return pauser.Select(x => x ? source : nothing).Switch();
        }
    }
}
