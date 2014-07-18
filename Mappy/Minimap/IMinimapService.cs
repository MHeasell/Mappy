namespace Mappy.Minimap
{
    using System.Drawing;

    public interface IMinimapService
    {
        void SetViewportCenterNormalized(PointF location);
    }
}
