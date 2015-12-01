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
            var subject = new BehaviorSubject<TField>(accessor(source));

            // FIXME: This is leaky.
            // We create a subscription to connect this to the subject
            // but we don't hold onto this subscription.
            // The subject we return can never be garbage collected
            // because the subscription cannot be freed.
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => source.PropertyChanged += x,
                x => source.PropertyChanged -= x)
                .Where(x => x.EventArgs.PropertyName == name)
                .Select(_ => accessor(source))
                .Subscribe(subject);

            return subject;
        }
    }
}
