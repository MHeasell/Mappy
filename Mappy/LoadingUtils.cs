namespace Mappy
{
    using System.Collections.Generic;
    using System.IO;

    public static class LoadingUtils
    {
        public static IEnumerable<string> EnumerateSearchHpis()
        {
            string[] exts = { "hpi", "ufo", "ccx", "gpf", "gp3" };
            if (MappySettings.Settings.SearchPaths == null)
            {
                yield break;
            }

            foreach (var ext in exts)
            {
                foreach (var dir in MappySettings.Settings.SearchPaths)
                {
                    if (!Directory.Exists(dir))
                    {
                        // silently ignore missing directories
                        continue;
                    }

                    foreach (var file in Directory.EnumerateFiles(dir, "*." + ext))
                    {
                        yield return file;
                    }
                }
            }
        }
    }
}
