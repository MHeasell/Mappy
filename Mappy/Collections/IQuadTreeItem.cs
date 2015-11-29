namespace Mappy.Collections
{
    using System.Drawing;

    public interface IQuadTreeItem
    {
        Rectangle Bounds { get; }
    }
}