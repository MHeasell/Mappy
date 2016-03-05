namespace Mappy.Models
{
    using System.Drawing;
    using System.ComponentModel;

    public interface IMinimapFormViewModel : INotifyPropertyChanged
    {
        bool MinimapVisible { get; }

        Maybe<Bitmap> MinimapImage { get; }

        Rectangle MinimapRect { get; }

        void MouseDown(Point location);

        void MouseMove(Point location);

        void MouseUp();

        void FormCloseButtonClick();
    }
}
