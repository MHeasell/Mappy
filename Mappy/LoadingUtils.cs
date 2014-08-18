namespace Mappy
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Data;

    using Mappy.IO;
    using Mappy.Palette;
    using Mappy.Util;

    using TAUtil.Gaf;
    using TAUtil.Hpi;
    using TAUtil.Sct;
    using TAUtil.Tdf;

    public static class LoadingUtils
    {
        /// <summary>
        /// Loads the database of features from disk
        /// </summary>
        /// <param name="palette"></param>
        /// <returns></returns>
        public static IDictionary<string, Feature> LoadFeatures(IPalette palette)
        {
            IDictionary<string, TdfNode> tdfs = LoadingUtils.LoadFeatureTdfs();
            IDictionary<string, GafFrame> frames = LoadingUtils.LoadFeatureBitmaps(tdfs);
            IDictionary<string, OffsetBitmap> renders = LoadingUtils.LoadFeatureRenders(tdfs);

            return LoadingUtils.LoadFeatureObjects(tdfs, frames, renders, palette);
        }

        public static IList<Section> LoadSections(IPalette palette)
        {
            IList<Section> sections = new List<Section>();
            int i = 0;
            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                foreach (Section s in LoadingUtils.LoadSectionsFromHapi(file, palette))
                {
                    s.Id = i++;
                    sections.Add(s);
                }
            }

            return sections;
        }

        private static Dictionary<string, Feature> LoadFeatureObjects(
            IDictionary<string, TdfNode> tdfs,
            IDictionary<string, GafFrame> frames,
            IDictionary<string, OffsetBitmap> objects,
            IPalette palette)
        {
            var maskedPalette = new TransparencyMaskedPalette(palette);
            var deserializer = new BitmapDeserializer(maskedPalette);

            Dictionary<string, Feature> features = new Dictionary<string, Feature>();
            foreach (var e in tdfs)
            {
                GafFrame frame;
                OffsetBitmap render;
                if (frames.TryGetValue(e.Key, out frame))
                {
                    int footX = Convert.ToInt32(e.Value.Entries["footprintx"]);
                    int footY = Convert.ToInt32(e.Value.Entries["footprintz"]);
                    Bitmap image;
                    if (frame.Width == 0 || frame.Height == 0)
                    {
                        image = new Bitmap(50, 50);
                    }
                    else
                    {
                        maskedPalette.TransparencyIndex = frame.TransparencyIndex;
                        image = deserializer.Deserialize(frame.Data, frame.Width, frame.Height);
                    }

                    Feature f = new Feature(e.Key, image, new Point(frame.OffsetX, frame.OffsetY), new Size(footX, footY));
                    f.World = e.Value.Entries["world"];
                    f.Category = e.Value.Entries["category"];
                    features[e.Key] = f;
                }
                else if (objects.TryGetValue(e.Key, out render))
                {
                    int footX = Convert.ToInt32(e.Value.Entries["footprintx"]);
                    int footY = Convert.ToInt32(e.Value.Entries["footprintz"]);
                    Feature f = new Feature(e.Key, render.Bitmap, new Point(-render.OffsetX, -render.OffsetY), new Size(footX, footY));
                    f.World = e.Value.Entries["world"];
                    f.Category = e.Value.Entries["category"];
                    features[e.Key] = f;
                }
            }

            return features;
        }

        private static IEnumerable<Section> LoadSectionsFromHapi(string filename, IPalette palette)
        {
            var factory = new SectionFactory(palette);

            using (HpiReader h = new HpiReader(filename))
            {
                foreach (string sect in h.GetFilesRecursive("sections").Select(x => x.Name))
                {
                    using (var s = new SctReader(h.ReadFile(sect)))
                    {
                        Section section = new Section(filename, sect);
                        section.Name = Path.GetFileNameWithoutExtension(sect);
                        section.Minimap = factory.MinimapFromSct(s);
                        section.DataWidth = s.DataWidth;
                        section.DataHeight = s.DataHeight;

                        string[] directories = Path.GetDirectoryName(sect).Split(Path.DirectorySeparatorChar);

                        section.World = directories[1];
                        section.Category = directories[2];

                        yield return section;
                    }
                }
            }
        }

        /// <summary>
        /// Loads the corresponding image for each feature in the given database
        /// </summary>
        /// <param name="features"></param>
        /// <returns>A mapping of feature name to image</returns>
        private static IDictionary<string, GafFrame> LoadFeatureBitmaps(IDictionary<string, TdfNode> features)
        {
            IDictionary<string, IList<TdfNode>> filenameFeatureMap = LoadingUtils.GroupByAnimFilename(features);

            Dictionary<string, GafFrame> bitmaps = new Dictionary<string, GafFrame>();

            if (MappySettings.Settings.SearchPaths == null)
            {
                return bitmaps;
            }

            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                using (HpiReader h = new HpiReader(file))
                {
                    foreach (var e in LoadFeatureBitmapsFromHapi(h, filenameFeatureMap))
                    {
                        bitmaps[e.Key] = e.Value;
                    }
                }
            }

            return bitmaps;
        }

        private static IDictionary<string, OffsetBitmap> LoadFeatureRenders(IDictionary<string, TdfNode> tdfs)
        {
            var renders = new Dictionary<string, OffsetBitmap>();

            var objectFeatureMap = GroupByField(tdfs, "object");

            if (MappySettings.Settings.SearchPaths == null)
            {
                return renders;
            }

            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                using (HpiReader h = new HpiReader(file))
                {
                    foreach (var objPath in h.GetFilesRecursive("objects3d").Select(x => x.Name))
                    {
                        var objName = Path.GetFileNameWithoutExtension(objPath).ToLowerInvariant();

                        IList<TdfNode> val;
                        if (objectFeatureMap.TryGetValue(objName, out val))
                        {
                            using (var b = h.ReadFile(objPath))
                            {
                                var r = new ModelEdgeReader();
                                r.Read(b);
                                var wire = Util.Util.RenderWireframe(r.Edges);
                                foreach (var item in val)
                                {
                                    renders[item.Name] = wire;
                                }
                            }
                        }
                    }
                }
            }

            return renders;
        }

        private static IEnumerable<string> EnumerateSearchHpis()
        {
            string[] exts = { "hpi", "ufo", "ccx", "gpf", "gp3" };
            if (MappySettings.Settings.SearchPaths == null)
            {
                yield break;
            }

            foreach (string ext in exts)
            {
                foreach (string dir in MappySettings.Settings.SearchPaths)
                {
                    if (!Directory.Exists(dir))
                    {
                        // silently ignore missing directories
                        continue;
                    }

                    foreach (string file in Directory.EnumerateFiles(dir, "*." + ext))
                    {
                        yield return file;
                    }
                }
            }
        }

        private static IDictionary<string, IList<TdfNode>> GroupByAnimFilename(IDictionary<string, TdfNode> features)
        {
            Dictionary<string, IList<TdfNode>> filenameFeatureMap = new Dictionary<string, IList<TdfNode>>();
            foreach (var e in features)
            {
                string filename;

                // skip features with no filename entry
                if (!e.Value.Entries.TryGetValue("filename", out filename))
                {
                    continue;
                }

                // normalize filenames
                filename = filename.ToLower();

                // try to retrieve existing list
                IList<TdfNode> l;
                if (!filenameFeatureMap.TryGetValue(filename, out l))
                {
                    // create one if it doesn't exist
                    l = new List<TdfNode>();
                    filenameFeatureMap[filename] = l;
                }

                // add this feature to the list for that filename
                l.Add(e.Value);
            }

            return filenameFeatureMap;
        }

        private static IDictionary<string, IList<TdfNode>> GroupByField(IDictionary<string, TdfNode> features, string field)
        {
            Dictionary<string, IList<TdfNode>> map = new Dictionary<string, IList<TdfNode>>();
            foreach (var e in features)
            {
                string fieldValue;

                // skip features with no filename entry
                if (!e.Value.Entries.TryGetValue(field, out fieldValue))
                {
                    continue;
                }

                // normalize filenames
                fieldValue = fieldValue.ToLower();

                // try to retrieve existing list
                IList<TdfNode> l;
                if (!map.TryGetValue(fieldValue, out l))
                {
                    // create one if it doesn't exist
                    l = new List<TdfNode>();
                    map[fieldValue] = l;
                }

                // add this feature to the list for that filename
                l.Add(e.Value);
            }

            return map;
        }

        private static IDictionary<string, GafFrame> LoadFeatureBitmapsFromHapi(HpiReader hapi, IDictionary<string, IList<TdfNode>> filenameFeatureMap)
        {
            Dictionary<string, GafFrame> bitmaps = new Dictionary<string, GafFrame>();

            // for each anim file in the HPI
            foreach (string anim in hapi.GetFilesRecursive("anims").Select(x => x.Name))
            {
                // if we require anims from this file
                IList<TdfNode> val;
                if (filenameFeatureMap.TryGetValue(Path.GetFileNameWithoutExtension(anim).ToLower(), out val))
                {
                    // extract and read the file
                    GafEntry[] gaf;
                    using (var b = new GafReader(hapi.ReadFile(anim)))
                    {
                        gaf = b.Read();
                    }

                    // retrieve all required entries
                    foreach (TdfNode n in val)
                    {
                        string seq;
                        if (n.Entries.TryGetValue("seqname", out seq))
                        {
                            foreach (GafEntry gafEntry in gaf)
                            {
                                if (gafEntry.Name.Equals(seq, StringComparison.OrdinalIgnoreCase))
                                {
                                    bitmaps[n.Name] = gafEntry.Frames[0];
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return bitmaps;
        }

        private static Dictionary<string, TdfNode> LoadFeatureTdfs()
        {
            Dictionary<string, TdfNode> features = new Dictionary<string, TdfNode>();

            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                using (HpiReader h = new HpiReader(file))
                {
                    foreach (var e in LoadingUtils.LoadFeaturesFromHapi(h))
                    {
                        features[e.Key] = e.Value;
                    }
                }
            }

            return features;
        }

        private static Dictionary<string, TdfNode> LoadFeaturesFromHapi(HpiReader hapi)
        {
            Dictionary<string, TdfNode> features = new Dictionary<string, TdfNode>();
            foreach (string feature in hapi.GetFilesRecursive("features").Select(x => x.Name))
            {
                if (feature.EndsWith(".tdf", StringComparison.OrdinalIgnoreCase))
                {
                    TdfNode n;
                    using (StreamReader sr = new StreamReader(hapi.ReadTextFile(feature)))
                    {
                        n = TdfNode.LoadTdf(sr);
                    }

                    foreach (var e in n.Keys)
                    {
                        features[e.Key] = e.Value;
                    }
                }
            }

            return features;
        }
    }
}
