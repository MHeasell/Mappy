namespace Mappy.IO
{
    using System;

    using Mappy.Data;

    public static class SectionLoadingUtils
    {
        public static bool LoadSections(
            Action<int> progressCallback,
            Func<bool> cancelCallback,
            out LoadResult<Section> result)
        {
            var loader = new SectionLoader();
            if (!loader.LoadFiles(progressCallback, cancelCallback))
            {
                result = null;
                return false;
            }

            result = loader.GetResult();
            return true;
        }
    }
}
