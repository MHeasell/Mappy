namespace TAUtil
{
    using System.Drawing;
    using System.IO;

    public static class Palette
    {
        public static Color[] LoadArr(Stream file)
        {
            StreamReader s = new StreamReader(file);
            string magic1 = s.ReadLine();
            string magic2 = s.ReadLine();
            if (!magic1.Equals("JASC-PAL")
                || !magic2.Equals("0100"))
            {
                throw new IOException("Unrecognised palette format");
            }

            string entries = s.ReadLine();
            if (!entries.Equals("256"))
            {
                throw new IOException("This palette is not 256 colors");
            }

            Color[] pal = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                string line = s.ReadLine();
                string[] fields = line.Split(' ');
                pal[i] = Color.FromArgb(
                    int.Parse(fields[0]),
                    int.Parse(fields[1]),
                    int.Parse(fields[2]));
            }

            return pal;
        }
    }
}
