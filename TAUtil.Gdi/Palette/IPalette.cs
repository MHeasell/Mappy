namespace TAUtil.Gdi.Palette
{
    using System.Collections.Generic;
    using System.Drawing;

    public interface IPalette : IEnumerable<Color>
    {
        int Count { get; }

        Color this[int index] { get; }

        int LookUp(Color color);
    }
}
