namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Util;

    public static class FeatureLoadingUtils
    {
        public static bool LoadFeatures(
            Action<int> progressCallback,
            Func<bool> cancelCallback,
            out LoadResult<FeatureInfo> result)
        {
            var recordLoader = new FeatureTdfLoader();
            if (!recordLoader.LoadFiles(progressCallback, cancelCallback))
            {
                result = null;
                return false;
            }

            var features = LoadFeatureObjects(recordLoader.Records).ToList();
            progressCallback(100);

            result = new LoadResult<FeatureInfo>
                {
                    Records = features,
                    Errors = recordLoader.HpiErrors,
                    FileErrors = recordLoader.FileErrors
                };
            return true;
        }

        private static IEnumerable<FeatureInfo> LoadFeatureObjects(
            IEnumerable<FeatureRecord> records)
        {
            foreach (var record in records)
            {
                var reclaimInfo = record.Reclaimable ?
                    Maybe.Return(new Feature.ReclaimInfoStruct
                    {
                        EnergyValue = record.Energy,
                        MetalValue = record.Metal
                    }) : Maybe.None<Feature.ReclaimInfoStruct>();

                yield return new FeatureInfo
                    {
                        Name = record.Name,
                        Footprint = new Size(record.FootprintX, record.FootprintY),
                        World = record.World,
                        Category = record.Category,

                        ReclaimInfo = reclaimInfo,
                        Permanent = record.Permanent,

                        AnimFileName = record.AnimFileName,
                        SequenceName = record.SequenceName,
                        ObjectName = record.ObjectName,
                    };
            }
        }
    }
}
