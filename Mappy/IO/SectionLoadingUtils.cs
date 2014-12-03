namespace Mappy.IO
{
    using System;

    using Mappy.Data;
    using Mappy.Palette;

    public static class SectionLoadingUtils
    {
        public static bool LoadSections(
            IPalette palette,
            Action<int> progressCallback,
            Func<bool> cancelCallback,
            out LoadResult<Section> result)
        {
            var loader = new SectionLoader(palette);
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
