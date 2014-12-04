namespace Mappy.Models
{
    using System;
    using System.ComponentModel;

    using Mappy.Collections;
    using Mappy.Data;

    public interface IBindingMapModel : IMapModel, INotifyPropertyChanged
    {
        event EventHandler<FeatureInstanceEventArgs> FeatureInstanceChanged;

        new BindingMapTile Tile { get; }

        new BindingList<Positioned<IMapTile>> FloatingTiles { get; }

        new BindingSparseGrid<bool> Voids { get; }
    }

    public class FeatureInstanceEventArgs : EventArgs
    {
        public enum ActionType
        {
            Add,
            Move,
            Remove
        }

        public FeatureInstanceEventArgs(ActionType action, Guid featureInstanceId)
        {
            this.Action = action;
            this.FeatureInstanceId = featureInstanceId;
        }

        public ActionType Action { get; set; }

        public Guid FeatureInstanceId { get; set; }
    }
}
