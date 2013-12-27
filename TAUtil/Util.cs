namespace TAUtil
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using TAUtil.Tnt;

    public static class Util
    {
        public static string ConvertChars(byte[] data)
        {
            int i = Array.IndexOf<byte>(data, 0);
            Debug.Assert(i != -1, "null terminator not found");
            return System.Text.Encoding.ASCII.GetString(data, 0, i);
        }

        public static Size GetMinimapActualSize(byte[] data, int width, int height)
        {
            int realHeight = 0;
            int realWidth = 0;

            for (int y = 0; y < height; y++)
            {
                if (data[y * width] == TntConstants.MinimapVoidByte)
                {
                    break;
                }

                realHeight++;
            }

            for (int x = 0; x < width; x++)
            {
                if (data[x] == TntConstants.MinimapVoidByte)
                {
                    break;
                }

                realWidth++;
            }

            return new Size(realWidth, realHeight);
        }

        public static byte[] TrimMinimapBytes(byte[] data, int width, int height, int newWidth, int newHeight)
        {
            byte[] newData = new byte[newWidth * newHeight];

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    newData[(y * newWidth) + x] = data[(y * width) + x];
                }
            }

            return newData;
        }

        public struct Size
        {
            public Size(int width, int height)
                : this()
            {
                this.Width = width;
                this.Height = height;
            }

            public int Width { get; private set; }

            public int Height { get; private set; }
        }
    }
}
