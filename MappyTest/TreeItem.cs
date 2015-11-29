namespace MappyTest
{
    using System.Drawing;

    using Mappy.Collections;

    public class TreeItem : IQuadTreeItem
    {
        public TreeItem(Rectangle bounds)
        {
            this.Bounds = bounds;
        }

        public Rectangle Bounds { get; private set; }
    }
}