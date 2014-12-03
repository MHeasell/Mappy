namespace Mappy.IO
{
    using System;

    public class HpiInnerFileErrorInfo
    {
        public string HpiPath { get; set; }

        public string FeaturePath { get; set; }

        public Exception Error { get; set; }
    }
}