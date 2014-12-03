namespace Mappy.IO
{
    using System.Collections.Generic;

    public class LoadResult<T>
    {
        public List<T> Records { get; set; }

        public List<HpiErrorInfo> Errors { get; set; }

        public List<HpiInnerFileErrorInfo> FileErrors { get; set; }
    }
}