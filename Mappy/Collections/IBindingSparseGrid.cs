namespace Mappy.Collections
{
    using System;
    using System.Collections.Generic;

    public interface IBindingSparseGrid<T> : ISparseGrid<T>, IBindingGrid<T>
    {
        event EventHandler<SparseGridEventArgs> EntriesChanged;
    }

    public class SparseGridEventArgs : EventArgs
    {
        public SparseGridEventArgs(ActionType action, IEnumerable<int> indexes)
        {
            this.Action = action;
            this.Indexes = indexes;
        }

        public enum ActionType
        {
            Set,
            Remove,
        }

        public ActionType Action { get; set; }

        public IEnumerable<int> Indexes { get; set; }
    }
}
