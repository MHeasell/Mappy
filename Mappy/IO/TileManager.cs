namespace Mappy.IO
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Security.Cryptography;
    using System.Text;

    using TAUtil;

    using Util = Mappy.Util.Util;

    public class TileManager
    {
        private static readonly SHA1 Sha = SHA1.Create();

        private readonly IDictionary<string, Bitmap> dictionary = new Dictionary<string, Bitmap>();

        private readonly Color[] palette;

        public TileManager(Color[] palette)
        {
            this.palette = palette;
        }

        public Bitmap GetOrCreateBitmap(byte[] tileData)
        {
            string hash = Encoding.ASCII.GetString(Sha.ComputeHash(tileData));
            Bitmap bmp;
            if (!this.dictionary.TryGetValue(hash, out bmp))
            {
                bmp = Util.ReadToBitmap(tileData, this.palette, MapConstants.TileWidth, MapConstants.TileHeight);
                this.dictionary[hash] = bmp;
            }

            return bmp;
        }
    }
}
