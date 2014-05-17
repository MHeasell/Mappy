namespace Mappy
{
    using System;
    using System.Globalization;

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

        public static string ToString(double d)
        {
            return Convert.ToString(d);
        }

        public static bool ToBool(string s)
        {
            return !(string.IsNullOrEmpty(s) || s == "0");
        }

        public static int ToInt32(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return 0;
            }

            return Convert.ToInt32(s);
        }

        public static double ToDouble(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return 0.0;
            }

            return Convert.ToDouble(s);
        }
    }
}
