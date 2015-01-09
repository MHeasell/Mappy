namespace Mappy.Controllers
{
    using System.Collections.Generic;

    using Mappy.Data;
    using Mappy.IO;

    public class SectionFeatureLoadResult
    {
        public IList<Section> Sections { get; set; }

        public IList<Feature> Features { get; set; }

        public List<HpiErrorInfo> Errors { get; set; }

        public List<HpiInnerFileErrorInfo> FileErrors { get; set; }
    }
}