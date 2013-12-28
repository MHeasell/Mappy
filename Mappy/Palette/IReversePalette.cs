namespace Mappy.Palette
{
    using System.Drawing;

    public interface IReversePalette
    {
        int this[Color color] { get; }
    }
}
