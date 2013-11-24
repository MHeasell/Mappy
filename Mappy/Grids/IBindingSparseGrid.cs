namespace Mappy.Grids
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public interface IBindingSparseGrid<T> : ISparseGrid<T>, IBindingGrid<T>
    {
        event EventHandler<SparseGridEventArgs> EntriesChanged;
    }

    public class SparseGridEventArgs : EventArgs
    {
        public enum ActionType
        {
            Set,
            Remove,
        }

        public ActionType Action { get; set; }

        public IEnumerable<Point> Coordinates { get; set; }
    }
}
