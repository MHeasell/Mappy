namespace Mappy.Util.ImageSampling
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    using Mappy.Collections;

    public sealed class MapPixelImageAdapter : IPixelImage, IDisposable
    {
        private readonly IGrid<Bitmap> map;

        private readonly Dictionary<Bitmap, BitmapData> dataMap = new Dictionary<Bitmap, BitmapData>();

        private bool disposed;

        public MapPixelImageAdapter(IGrid<Bitmap> map)
        {
            this.map = map;
        }

        ~MapPixelImageAdapter()
        {
            this.Dispose(false);
        }

        public int Width => (this.map.Width * 32) - 32;

        public int Height => (this.map.Height * 32) - 128;

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
                // Free managed resoures here.
                // We currently have no managed resources to free.
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
