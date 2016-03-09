namespace Mappy.Models
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.ComponentModel;
    using System.Reactive.Subjects;

    public interface IMinimapFormViewModel : INotifyPropertyChanged
    {
        bool MinimapVisible { get; }

        Maybe<Bitmap> MinimapImage { get; }

        Rectangle MinimapRect { get; }

        // TODO: replace with IReadOnlyList<IObservable...> after updating to .NET 4.5
        IList<BehaviorSubject<Maybe<Point>>> StartPositions { get; }

        void MouseDown(Point location);

        void MouseMove(Point location);

        void MouseUp();

        void FormCloseButtonClick();
    }
}
