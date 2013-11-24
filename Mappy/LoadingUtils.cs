namespace Mappy
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using Data;

    using Models;

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
        public static IDictionary<string, Feature> LoadFeatures(Color[] palette)
        {
            IDictionary<string, TdfNode> tdfs = LoadingUtils.LoadFeatureTdfs();
            IDictionary<string, GafFrame> frames = LoadingUtils.LoadFeatureBitmaps(tdfs, palette);

            return LoadingUtils.LoadFeatureObjects(tdfs, frames);
        }

        public static IList<Section> LoadSections(Color[] palette)
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

        private static Dictionary<string, Feature> LoadFeatureObjects(IDictionary<string, TdfNode> tdfs, IDictionary<string, GafFrame> frames)
        {
            Dictionary<string, Feature> features = new Dictionary<string, Feature>();
            foreach (var e in tdfs)
            {
                GafFrame frame;
                if (frames.TryGetValue(e.Key, out frame))
                {
                    int footX = Convert.ToInt32(e.Value.Entries["footprintx"]);
                    int footY = Convert.ToInt32(e.Value.Entries["footprintz"]);
                    Feature f = new Feature(e.Key, frame.Data, frame.Offset, new Size(footX, footY));
                    f.World = e.Value.Entries["world"];
                    f.Category = e.Value.Entries["category"];
                    features[e.Key] = f;
                }
            }

            return features;
        }

        private static IEnumerable<Section> LoadSectionsFromHapi(string filename, Color[] palette)
        {
            using (HpiReader h = new HpiReader(filename))
            {
                foreach (string sect in h.GetFilesRecursive("sections"))
                {
                    using (Stream s = h.ReadFile(sect))
                    {
                        SctFile sctFile = new SctFile(s);

                        Section section = new Section(filename, sect, palette);
                        section.Name = Path.GetFileNameWithoutExtension(sect);
                        section.Minimap = sctFile.GetMinimapBitmap(palette);

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
        /// <param name="palette"></param>
        /// <returns>A mapping of feature name to image</returns>
        private static IDictionary<string, GafFrame> LoadFeatureBitmaps(IDictionary<string, TdfNode> features, Color[] palette)
        {
            IDictionary<string, IList<TdfNode>> filenameFeatureMap = LoadingUtils.GroupByAnimFilename(features);

            Dictionary<string, GafFrame> bitmaps = new Dictionary<string, GafFrame>();

            if (Properties.Settings.Default.SearchDirectories == null)
            {
                return bitmaps;
            }

            foreach (string file in LoadingUtils.EnumerateSearchHpis())
            {
                using (HpiReader h = new HpiReader(file))
                {
                    foreach (var e in LoadFeatureBitmapsFromHapi(h, filenameFeatureMap, palette))
                    {
                        bitmaps[e.Key] = e.Value;
                    }
                }
            }

            return bitmaps;
        }

        private static IEnumerable<string> EnumerateSearchHpis()
        {
            string[] exts = new string[] { "hpi", "ufo", "ccx", "gpf", "gp3" };
            if (Properties.Settings.Default.SearchDirectories == null)
            {
                yield break;
            }

            foreach (string ext in exts)
            {
                foreach (string dir in Properties.Settings.Default.SearchDirectories)
                {
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

        private static IDictionary<string, GafFrame> LoadFeatureBitmapsFromHapi(HpiReader hapi, IDictionary<string, IList<TdfNode>> filenameFeatureMap, Color[] palette)
        {
            Dictionary<string, GafFrame> bitmaps = new Dictionary<string, GafFrame>();

            // for each anim file in the HPI
            foreach (string anim in hapi.GetFilesRecursive("anims"))
            {
                // if we require anims from this file
                IList<TdfNode> val;
                if (filenameFeatureMap.TryGetValue(Path.GetFileNameWithoutExtension(anim).ToLower(), out val))
                {
                    // extract and read the file
                    GafEntry[] gaf;
                    using (Stream s = hapi.ReadFile(anim))
                    {
                        gaf = GafFile.Read(s, palette);
                    }

                    // retrieve all required entries
                    foreach (TdfNode n in val)
                    {
                        string seq = n.Entries["seqname"];
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
            foreach (string feature in hapi.GetFilesRecursive("features"))
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
