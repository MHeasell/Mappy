namespace Mappy.Palette
{
    using System.Drawing;

    public interface IPalette
    {
        Color this[int index] { get; }
    }
}
