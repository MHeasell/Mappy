namespace Mappy.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using Hjg.Pngcs;
    using Hjg.Pngcs.Chunks;

    using Mappy.Collections;
    using Mappy.Data;
    using Mappy.Services;

    using TAUtil.Gdi.Palette;

    using PixelFormat = System.Drawing.Imaging.PixelFormat;

    public class ImageImportService
    {
        private readonly BitmapCache tileCache;

        public ImageImportService(BitmapCache tileCache)
        {
            this.tileCache = tileCache;
        }

        public IMapTile ImportSection(string pngFile, string heightFile, Action<int> progress, Func<bool> shouldCancel)
        {
            var heightBitmap = Util.BitmapFromFile(heightFile);

            Grid<Bitmap> grid;
            using (var s = File.OpenRead(pngFile))
            {
                var reader = new PngReader(s);
                if (reader.ImgInfo.BitDepth != 8)
                {
                    throw new ArgumentException(
                        "Only 8-bit RGBA PNG files are supported");
                }

                if (reader.ImgInfo.Cols != heightBitmap.Width * 16
                    || reader.ImgInfo.Rows != heightBitmap.Height * 16)
                {
                    throw new ArgumentException(
                        "Graphic image dimensions are not a multiple of the heightmap");
                }

                grid = this.ImportFromPng(reader, progress, shouldCancel);

                if (grid == null)
                {
                    return null;
                }
            }

            var heightmap = Util.ReadHeightmap(heightBitmap);

            return new MapTile(grid, heightmap);
        }

        public Grid<Bitmap> ImportFromPng(PngReader reader, Action<int> progress, Func<bool> shouldCancel)
        {
            var w = reader.ImgInfo.Cols / 32;
            var h = reader.ImgInfo.Rows / 32;
            var g = new Grid<Bitmap>(w, h);

            for (var y = 0; y < h; y++)
            {
                if (shouldCancel())
                {
                    return null;
                }

                var x = 0;
                foreach (var bmp in this.ReadRowChunk(reader, y))
                {
                    g.Set(x++, y, bmp);
                }

                progress((100 * (y + 1)) / h);
            }

            return g;
        }

        private static unsafe Bitmap ReadBitmap(int bitmapIndex, byte[][] buf, PngChunkPLTE pal)
        {
            var bmp = new Bitmap(32, 32, PixelFormat.Format32bppArgb);

            var data = bmp.LockBits(
                new Rectangle(0, 0, 32, 32),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                var ptr = (int*)data.Scan0;

                for (var y = 0; y < 32; y++)
                {
                    var line = buf[y];

                    for (var x = 0; x < 32; x++)
                    {
                        var offset = (bitmapIndex * 32) + x;
                        var bmpOffset = (y * 32) + x;
                        int idx = line[offset];

                        var rgb = new int[3];
                        pal.GetEntryRgb(idx, rgb);

                        var c = Color.FromArgb(rgb[0], rgb[1], rgb[2]);

                        if (!PaletteFactory.TAPalette.Contains(c))
                        {
                            throw new ArgumentException("Image contains colors not in the TA palette.");
                        }

                        ptr[bmpOffset] = c.ToArgb();
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            return bmp;
        }

        private IEnumerable<Bitmap> ReadRowChunk(PngReader reader, int chunkIndex)
        {
            if (!reader.ImgInfo.Indexed)
            {
                throw new ArgumentException("Only indexed color PNGs are supported.");
            }

            var lines = new byte[32][];
            var pal = reader.GetMetadata().GetPLTE();

            for (var i = 0; i < 32; i++)
            {
                lines[i] = reader.ReadRowByte(
                    new byte[reader.ImgInfo.Cols],
                    (chunkIndex * 32) + i);
            }

            var bitmapsPerRow = reader.ImgInfo.Cols / 32;
            for (var i = 0; i < bitmapsPerRow; i++)
            {
                var bmp = ReadBitmap(i, lines, pal);
                this.tileCache.GetOrAddBitmap(bmp);
                yield return bmp;
            }
        }
    }
}
