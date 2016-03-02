namespace Mappy.Util
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public abstract class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
