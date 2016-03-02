namespace Mappy.Util.ImageSampling
{
    using System.Drawing;

    public interface IPixelImage
    {
        int Width { get; }

        int Height { get; }

        Color GetPixel(int x, int y);
    }
}
