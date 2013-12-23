namespace TAUtil
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public static class Util
    {
        public static string ConvertChars(byte[] data)
        {
            int i = Array.IndexOf<byte>(data, 0);
            Debug.Assert(i != -1, "null terminator not found");
            return System.Text.Encoding.ASCII.GetString(data, 0, i);
        }
    }
}
