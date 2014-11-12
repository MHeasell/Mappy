namespace Mappy.Models
{
    using System;

    [Serializable]
    public class FeatureClipboardRecord
    {
        public FeatureClipboardRecord(string featureName)
        {
            this.FeatureName = featureName;
        }

        public string FeatureName { get; set; }
    }
}
