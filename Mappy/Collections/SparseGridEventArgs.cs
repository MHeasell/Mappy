namespace Mappy.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Arguments to sparse grid cell events.
    /// </summary>
    public class SparseGridEventArgs : EventArgs
    {
        public SparseGridEventArgs(ActionType action, IEnumerable<int> oldIndexes, IEnumerable<int> indexes)
        {
            this.Action = action;
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
        /// Gets or sets the indices of the items before they were moved.
        /// Only set when the operation is a move.
        /// </summary>
        public IEnumerable<int> OriginalIndexes { get; set; }

        public IEnumerable<int> Indexes { get; set; }

        public static SparseGridEventArgs Move(int oldIndex, int newIndex)
        {
            return new SparseGridEventArgs(
                ActionType.Move,
                new[] { oldIndex },
                new[] { newIndex });
        }

        public static SparseGridEventArgs Set(int index)
        {
            return new SparseGridEventArgs(
                ActionType.Set,
                null,
                new[] { index });
        }

        public static SparseGridEventArgs Remove(int index)
        {
            return new SparseGridEventArgs(
                ActionType.Remove,
                null,
                new[] { index });
        }
    }
}