namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Security.Cryptography;
    using System.Text;

    using TAUtil;

    using Util = Mappy.Util.Util;

    public class TileManager
    {
        private static readonly SHA1 Sha = SHA1.Create();

        private readonly IDictionary<string, WeakReference> dictionary = new Dictionary<string, WeakReference>();

        private readonly Color[] palette;

        public TileManager(Color[] palette)
        {
            this.palette = palette;
        }

        public Bitmap GetOrCreateBitmap(byte[] tileData)
        {
            string hash = Encoding.ASCII.GetString(Sha.ComputeHash(tileData));

            WeakReference weakRef;
            if (!this.dictionary.TryGetValue(hash, out weakRef))
            {
                // we didn't find the ref, so generate one
                Bitmap bmp = this.GenerateBitmap(tileData);
                this.dictionary[hash] = new WeakReference(bmp);
                return bmp;
            }

            // we found a ref, see if we still have the bitmap
            Bitmap b = weakRef.Target as Bitmap;
            if (b == null)
            {
                // it's gone, generate a new one
                b = this.GenerateBitmap(tileData);
                weakRef.Target = b;
            }

            return b;
        }

        private Bitmap GenerateBitmap(byte[] tileData)
        {
            return Util.ReadToBitmap(tileData, this.palette, MapConstants.TileWidth, MapConstants.TileHeight);
        }
    }
}
