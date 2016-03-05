namespace Mappy
{
    using System;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reactive.Linq;

    public static class ExtensionMethods
    {
        public static IObservable<TField> PropertyAsObservable<TSource, TField>(this TSource source, Func<TSource, TField> accessor, string name)
            where TSource : INotifyPropertyChanged
        {
            return Observable.Create<TField>(
                observer =>
                    {
                        observer.OnNext(accessor(source));
                        var values = Observable
                            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                x => source.PropertyChanged += x,
                                x => source.PropertyChanged -= x)
                            .Where(x => x.EventArgs.PropertyName == name)
                            .Select(_ => accessor(source));
                        return values.Subscribe(observer);
                    });
        }

        public static IObservable<Unit> PropertyChangedObservable<T>(this T source, string name)
            where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    x => source.PropertyChanged += x,
                    x => source.PropertyChanged -= x)
                .Where(x => x.EventArgs.PropertyName == name)
                .Select(_ => Unit.Default);
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

        /// <summary>
        /// Creates an observable that streams the value of the selected property
        /// of the value contained in source.
        /// If the source becomes null, the default value is emitted instead.
        /// </summary>
        public static IObservable<TValue> ObservePropertyOrDefault<TSource, TValue>(
            this IObservable<TSource> source,
            Func<TSource, TValue> accessor,
            string name,
            TValue defaultValue)
            where TSource : INotifyPropertyChanged
        {
            var defaultObservable = Observable.Return(defaultValue);
            return source.Select(x => x?.PropertyAsObservable(accessor, name) ?? defaultObservable)
                .Switch();
        }

        /// <summary>
        /// Creates an observable that streams the value of the selected property
        /// of the value contained in source.
        /// If the source becomes None, the default value is emitted instead.
        /// </summary>
        public static IObservable<TValue> ObservePropertyOrDefault<TSource, TValue>(
            this IObservable<Maybe<TSource>> source,
            Func<TSource, TValue> accessor,
            string name,
            TValue defaultValue)
            where TSource : INotifyPropertyChanged
        {
            var defaultObservable = Observable.Return(defaultValue);
            return source.Select(x => x.Match(y => y.PropertyAsObservable(accessor, name), () => defaultObservable))
                .Switch();
        }

        public static Maybe<T> ToMaybe<T>(this T? item)
            where T : struct
        {
            return Maybe.From(item);
        }
    }
}
