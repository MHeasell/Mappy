namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;

    using Mappy.Data;
    using Mappy.Util;

    using TAUtil.Gaf;
    using TAUtil.Gdi.Bitmap;
    using TAUtil.Gdi.Palette;
    using TAUtil.Hpi;

    public class FeatureBitmapLoader : AbstractHpiLoader<KeyValuePair<string, OffsetBitmap>>
    {
        private readonly IDictionary<string, IList<FeatureRecord>> filenameFeatureMap;

        private readonly BitmapDeserializer deserializer;

        private readonly TransparencyMaskedPalette palette;

        public FeatureBitmapLoader(IPalette palette, IDictionary<string, IList<FeatureRecord>> filenameFeatureMap)
        {
            this.filenameFeatureMap = filenameFeatureMap;
            this.palette = new TransparencyMaskedPalette(palette);
            this.deserializer = new BitmapDeserializer(this.palette);
        }

        protected override IEnumerable<string> EnumerateFiles(HpiReader r)
        {
            return r.GetFilesRecursive("anims")
                .Select(x => x.Name)
                .Where(this.IsNeededFile);
        }

        protected override void LoadFile(HpiReader r, string file)
        {
            // extract and read the file
            GafEntry[] gaf;
            using (var b = new GafReader(r.ReadFile(file)))
            {
                gaf = b.Read();
            }

            Debug.Assert(file != null, "Null path in HPI listing.");

            var records = this.filenameFeatureMap[HpiPath.GetFileNameWithoutExtension(file)];

            // retrieve the anim for each record
            foreach (var record in records)
            {
                var sequenceName = record.SequenceName;
                if (string.IsNullOrEmpty(sequenceName))
                {
                    // Skip if this record has no sequence name.
                    continue;
                }

                var entry = gaf.FirstOrDefault(
                    x => string.Equals(x.Name, sequenceName, StringComparison.OrdinalIgnoreCase));
                if (entry == null)
                {
                    // skip if the sequence is not in this gaf file
                    continue;
                }

                var frame = entry.Frames[0];
                this.palette.TransparencyIndex = frame.TransparencyIndex;

                Bitmap bmp;
                if (frame.Width == 0 || frame.Height == 0)
                {
                    bmp = new Bitmap(50, 50);
                }
                else
                {
                    bmp = this.deserializer.Deserialize(frame.Data, frame.Width, frame.Height);
                }

                var offsetImage = new OffsetBitmap(-frame.OffsetX, -frame.OffsetY, bmp);
                this.Records.Add(new KeyValuePair<string, OffsetBitmap>(record.Name, offsetImage));
            }
        }

        private bool IsNeededFile(string file)
        {
            return file.EndsWith(".gaf", StringComparison.OrdinalIgnoreCase)
                && this.filenameFeatureMap.ContainsKey(
                    HpiPath.GetFileNameWithoutExtension(file));
        }
    }
}
