namespace TAUtil.Gaf
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class GafFile
    {
        public static GafEntry[] Read(BinaryReader b, Color[] palette)
        {
            // read in header
            Structures.GafHeader header = new Structures.GafHeader();
            Structures.GafHeader.Read(b, ref header);

            // read in pointers to entries
            int[] pointers = new int[header.Entries];
            for (int i = 0; i < header.Entries; i++)
            {
                pointers[i] = b.ReadInt32();
            }

            // read in the actual entries themselves
            GafEntry[] entries = new GafEntry[header.Entries];
            for (int i = 0; i < header.Entries; i++)
            {
                b.BaseStream.Seek(pointers[i], SeekOrigin.Begin);
                entries[i] = GafFile.ReadGafEntry(b, palette);
            }

            return entries;
        }

        private static GafEntry ReadGafEntry(BinaryReader b, Color[] palette)
        {
            // read the entry header
            Structures.GafEntry entry = new Structures.GafEntry();
            Structures.GafEntry.Read(b, ref entry);

            // read in all the frame entry pointers
            Structures.GafFrameEntry[] frameEntries = new Structures.GafFrameEntry[entry.Frames];
            for (int i = 0; i < entry.Frames; i++)
            {
                Structures.GafFrameEntry.Read(b, ref frameEntries[i]);
            }

            // read in the corresponding frames
            GafFrame[] frames = new GafFrame[entry.Frames];

            for (int i = 0; i < entry.Frames; i++)
            {
                b.BaseStream.Seek(frameEntries[i].PtrFrameTable, SeekOrigin.Begin);
                frames[i] = GafFile.LoadFrame(b, palette);
            }

            // fill in and return our output structure
            GafEntry outEntry = new GafEntry();
            outEntry.Name = entry.Name;
            outEntry.Frames = frames;
            return outEntry;
        }

        private static GafFrame LoadFrame(BinaryReader b, Color[] palette)
        {
            // read in the frame data table
            Structures.GafFrameData d = new Structures.GafFrameData();
            Structures.GafFrameData.Read(b, ref d);

            GafFrame frame = new GafFrame();
            frame.Offset = new Point(d.XPos, d.YPos);

            // Some GAFs (e.g. GZMetalEnergyCrystals.gaf in tamec 2004)
            // have frames with zero width and height.
            // Don't try and load these.
            if (d.Width < 1 || d.Height < 1)
            {
                frame.Data = new Bitmap(1, 1);
                return frame;
            }

            // read the actual frame image
            b.BaseStream.Seek(d.PtrFrameData, SeekOrigin.Begin);

            if (d.FramePointers > 0)
            {
                // TODO: implement support for subframes
                frame.Data = new Bitmap(50, 50);
            }
            else
            {
                if (d.Compressed)
                {
                    frame.Data = GafFile.ReadCompressedImage(b, d.Width, d.Height, palette);
                }
                else
                {
                    frame.Data = GafFile.ReadUncompressedImage(b, d.Width, d.Height, palette);
                }
            }

            return frame;
        }

        private static Bitmap ReadUncompressedImage(BinaryReader b, int width, int height, Color[] palette)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                int* pointer = (int*)bitmapData.Scan0;
                int count = width * height;
                for (int i = 0; i < count; ++i)
                {
                    byte read = b.ReadByte();
                    if (read == 9)
                    {
                        pointer[i] = Color.Transparent.ToArgb();
                    }
                    else
                    {
                        pointer[i] = palette[read].ToArgb();
                    }
                }
            }

            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        private static Bitmap ReadCompressedImage(BinaryReader b, int width, int height, Color[] palette)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                int* pointer = (int*)data.Scan0;

                for (int y = 0; y < height; y++)
                {
                    int bytes = b.ReadUInt16();
                    int count = 0;
                    int x = 0;

                    // while there are still bytes left in the line to read
                    while (count < bytes)
                    {
                        // read the mask
                        byte mask = b.ReadByte();
                        count++;

                        if ((mask & 0x01) == 0x01)
                        {
                            // skip n pixels (transparency)
                            x += mask >> 1;
                        }
                        else if ((mask & 0x02) == 0x02)
                        {
                            // repeat this byte n times
                            byte next = b.ReadByte();
                            count++;

                            int repeat = (mask >> 2) + 1;
                            for (int i = 0; i < repeat; i++)
                            {
                                int pos = (y * width) + x;
                                pointer[pos] = palette[next].ToArgb();

                                x++;
                            }
                        }
                        else
                        {
                            // copy next n bytes
                            int repeat = (mask >> 2) + 1;
                            for (int i = 0; i < repeat; i++)
                            {
                                byte val = b.ReadByte();
                                count++;

                                int pos = (y * width) + x;
                                pointer[pos] = palette[val].ToArgb();

                                x++;
                            }
                        }
                    }
                }
            }

            bitmap.UnlockBits(data);
            return bitmap;
        } 
    }
}
