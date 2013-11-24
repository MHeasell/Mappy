namespace Mappy.Data
{
    using System;

    public class StartPositionChangedEventArgs : EventArgs
    {
        public StartPositionChangedEventArgs(int index)
        {
            this.Index = index;
        }

        public int Index { get; set; }
    }
}