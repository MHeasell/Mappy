namespace MappyTest
{
    using System.Drawing;

    using Mappy.Collections;

    public class TreeItem : QuadTree<TreeItem>.IQuadTreeItem
    {
        public TreeItem(Rectangle bounds)
        {
            this.Bounds = bounds;
        }

        public Rectangle Bounds { get; private set; }
    }
}