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

        public FeatureClipboardRecord(string featureName, int viewPortOffsetX, int viewPortOffsetY)
        {
            this.FeatureName = featureName;
            this.VPOffsetX = viewPortOffsetX;
            this.VPOffsetY = viewPortOffsetY;
        }

        public string FeatureName { get; set; }

        public int VPOffsetX { get; set; }

        public int VPOffsetY { get; set; }
    }
}
