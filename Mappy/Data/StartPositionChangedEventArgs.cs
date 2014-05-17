namespace Mappy.Data
{
    using System;

    /// <summary>
    /// Event arguments for start position change events.
    /// </summary>
    public class StartPositionChangedEventArgs : EventArgs
    {
        public StartPositionChangedEventArgs(int index)
        {
            this.Index = index;
        }

        public int Index { get; set; }
    }
}