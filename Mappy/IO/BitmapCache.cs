namespace Mappy.IO
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    /// <summary>
    /// Cache for storing bitmap data with de-duplication.
    /// Only one unique instance of an image will be stored.
    /// If an identical image is given, the original will be returned.
    /// </summary>
    public class BitmapCache
    {
        private static readonly SHA1 Sha = SHA1.Create();

        private readonly IDictionary<string, WeakReference> dictionary = new Dictionary<string, WeakReference>();

        public Bitmap GetOrAddBitmap(Bitmap bitmap)
        {
            string hash = ComputeHash(bitmap);

            WeakReference weakRef;
            if (!this.dictionary.TryGetValue(hash, out weakRef))
            {
                // we didn't find the ref, so add one
                this.dictionary[hash] = new WeakReference(bitmap);
                return bitmap;
            }

            // we found a ref, see if we still have the bitmap
            Bitmap b = weakRef.Target as Bitmap;
            if (b == null)
            {
                // it's gone, replace it with the one we were given
                weakRef.Target = bitmap;
                return bitmap;
            }

            // If we got here,
            // we managed to retrieve a live bitmap from cache
            return b;
        }

        private static string ComputeHash(Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

            int length = bitmap.Width * bitmap.Height * sizeof(int);
            byte[] managedData = new byte[length];

            Marshal.Copy(data.Scan0, managedData, 0, length);

            bitmap.UnlockBits(data);

            return Convert.ToBase64String(Sha.ComputeHash(managedData));
        }
    }
}
