namespace Mappy.Models
{
    using System;

    public class FeatureInstanceEventArgs : EventArgs
    {
        public FeatureInstanceEventArgs(ActionType action, Guid featureInstanceId)
        {
            this.Action = action;
            this.FeatureInstanceId = featureInstanceId;
        }

        public enum ActionType
        {
            Add,
            Move,
            Remove
        }

        public ActionType Action { get; private set; }

        public Guid FeatureInstanceId { get; private set; }
    }
}