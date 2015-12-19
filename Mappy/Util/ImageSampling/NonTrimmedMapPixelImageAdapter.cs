namespace Mappy.Util.ImageSampling
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    using Mappy.Collections;

    public sealed class NonTrimmedMapPixelImageAdapter : IPixelImage, IDisposable
    {
        private readonly IGrid<Bitmap> map;

        private readonly Dictionary<Bitmap, BitmapData> dataMap = new Dictionary<Bitmap, BitmapData>();

        private bool disposed;

        public NonTrimmedMapPixelImageAdapter(IGrid<Bitmap> map)
        {
            this.map = map;
        }

        ~NonTrimmedMapPixelImageAdapter()
        {
            this.Dispose(false);
        }

        public int Width
        {
            get
            {
                return this.map.Width * 32;
            }
        }

        public int Height
        {
            get
            {
                return this.map.Height * 32;
            }
        }

        public Color this[int x, int y]
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(null);
                }

                int tileX = x / 32;
                int tileY = y / 32;

                int tilePixelX = x % 32;
                int tilePixelY = y % 32;

                Bitmap bitmap = this.map[tileX, tileY];
                var data = this.GetOrLockData(bitmap);

                unsafe
                {
                    var ptr = (int*)data.Scan0;
                    var color = ptr[(tilePixelY * 32) + tilePixelX];
                    return Color.FromArgb(color);
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed resources here.
                // We currently have no managed resources to dispose.
            }

            foreach (var e in this.dataMap)
            {
                e.Key.UnlockBits(e.Value);
            }

            this.dataMap.Clear();

            this.disposed = true;
        }

        private BitmapData GetOrLockData(Bitmap bmp)
        {
            BitmapData data;
            if (!this.dataMap.TryGetValue(bmp, out data))
            {
                data = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);
                this.dataMap[bmp] = data;
            }

            return data;
        }
    }
}
