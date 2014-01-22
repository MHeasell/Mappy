namespace Mappy.Collections
{
    using System;
    using System.Collections.Generic;

    public interface IBindingSparseGrid<T> : ISparseGrid<T>, IBindingGrid<T>
    {
        event EventHandler<SparseGridEventArgs> EntriesChanged;

        void Move(int oldIndex, int newIndex);

        void Move(int srcX, int srcY, int destX, int destY);
    }

    public class SparseGridEventArgs : EventArgs
    {
        public SparseGridEventArgs(ActionType action, IEnumerable<int> indexes)
        {
            this.Action = action;
            this.Indexes = indexes;
        }

        public SparseGridEventArgs(IEnumerable<int> oldIndexes, IEnumerable<int> indexes)
        {
            this.Action = ActionType.Move;
            this.OriginalIndexes = oldIndexes;
            this.Indexes = indexes;
        }

        public enum ActionType
        {
            Set,
            Move,
            Remove,
        }

        public ActionType Action { get; set; }

        /// <summary>
        /// Only set when the operation is a move.
        /// </summary>
        public IEnumerable<int> OriginalIndexes { get; set; } 

        public IEnumerable<int> Indexes { get; set; }
    }
}
