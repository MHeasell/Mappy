namespace Mappy
{
    using System;

    public static class TdfConvert
    {
        public static string ToString(int i)
        {
            return Convert.ToString(i);
        }

        public static string ToString(bool b)
        {
            return b ? "1" : "0";
        }

        public static bool ToBool(string s)
        {
            return !(string.IsNullOrEmpty(s) || s == "0");
        }

        public static int ToInt(string s)
        {
            return Convert.ToInt32(s);
        }
    }
}
