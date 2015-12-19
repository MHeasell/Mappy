namespace Mappy.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Integer based QuadTree for storing items
    /// defined by a bounding rectangle.
    /// QuadTree provides fast quering of elements intersecting a point
    /// or rectangle.
    /// </summary>
    /// <typeparam name="T">The type of the elements</typeparam>
    public class QuadTree<T> : ICollection<T>
        where T : IQuadTreeItem
    {
        private const int TopLeftIndex = 0;

        private const int TopRightIndex = 1;

        private const int BottomLeftIndex = 2;

        private const int BottomRightIndex = 3;

        private const int DefaultSplitThreshold = 4;

        private readonly int splitThreshold;

        private readonly Rectangle bounds;

        private readonly LinkedList<T> items = new LinkedList<T>();

        private readonly QuadTree<T>[] nodes = new QuadTree<T>[4];

        public QuadTree(Rectangle bounds)
            : this(bounds, DefaultSplitThreshold)
        {
        }

        public QuadTree(Rectangle bounds, int splitThreshold)
        {
            this.bounds = bounds;
            this.splitThreshold = splitThreshold;
        }

        public QuadTree(Rectangle bounds, IEnumerable<T> collection)
            : this(bounds, collection, DefaultSplitThreshold)
        {
        }

        public QuadTree(Rectangle bounds, IEnumerable<T> collection, int splitThreshold)
            : this(bounds, splitThreshold)
        {
            foreach (T item in collection)
            {
                this.Add(item);
            }
        }

        public int Count { get; private set; }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        private bool HasSplit
        {
            get
            {
                return this.nodes[0] != null;
            }
        }

        public void Add(T item)
        {
            // If we have split, try to put this item into a subnode.
            if (this.HasSplit)
            {
                int index = this.DetermineRegion(item.Bounds);
                if (index != -1)
                {
                    this.nodes[index].Add(item);
                    this.Count++;
                    return;
                }
            }

            // We made it here, so either it did not fit
            // or we haven't split yet.
            // Put the item in our bin.
            this.items.AddLast(item);
            this.Count++;

            // Check whether we should split
            if (this.Count > this.splitThreshold && !this.HasSplit)
            {
                this.Split();

                // try and move existing items into the new subnodes
                var curNode = this.items.First;
                while (curNode != null)
                {
                    var nextNode = curNode.Next;

                    int index = this.DetermineRegion(curNode.Value.Bounds);
                    if (index != -1)
                    {
                        this.nodes[index].Add(curNode.Value);
                        this.items.Remove(curNode);
                    }

                    curNode = nextNode;
                }
            }
        }

        public void Clear()
        {
            this.UnSplit();

            this.items.Clear();

            this.Count = 0;
        }

        public bool Contains(T item)
        {
            if (this.HasSplit)
            {
                int index = this.DetermineRegion(item.Bounds);
                if (index != -1)
                {
                    return this.nodes[index].Contains(item);
                }
            }

            return this.items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if (array.Length - arrayIndex < this.Count)
            {
                throw new ArgumentException(
                    "number of elements to be copied exceeds the space available in the array");
            }

            int i = 0;
            foreach (T item in this)
            {
                array[arrayIndex + i] = item;
                i++;
            }
        }

        public bool Remove(T item)
        {
            bool success = false;
            bool inSubTree = false;
            if (this.HasSplit)
            {
                int index = this.DetermineRegion(item.Bounds);
                if (index != -1)
                {
                    inSubTree = true;
                    success = this.nodes[index].Remove(item);
                }
            }

            if (!inSubTree)
            {
                success = this.items.Remove(item);
            }

            if (success)
            {
                this.Count--;
            }

            if (this.Count <= this.splitThreshold && this.HasSplit)
            {
                foreach (QuadTree<T> t in this.nodes)
                {
                    foreach (T i in t)
                    {
                        this.items.AddLast(i);
                    }
                }

                this.UnSplit();
            }

            return success;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (this.HasSplit)
            {
                foreach (QuadTree<T> tree in this.nodes)
                {
                    foreach (T item in tree)
                    {
                        yield return item;
                    }
                }
            }

            foreach (T item in this.items)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerable<T> FindInArea(Rectangle rect)
        {
            foreach (T x in this.items.Where(x => x.Bounds.IntersectsWith(rect)))
            {
                yield return x;
            }

            if (this.HasSplit)
            {
                // If the area fits into a bin, go into that one.
                // Otherwise, enumerate over all bins.
                int index = this.DetermineRegion(rect);
                if (index != -1)
                {
                    foreach (T item in this.nodes[index].FindInArea(rect))
                    {
                        yield return item;
                    }
                }
                else
                {
                    foreach (QuadTree<T> tree in this.nodes)
                    {
                        foreach (T item in tree.FindInArea(rect))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        public IEnumerable<T> FindAtPoint(Point p)
        {
            foreach (T x in this.items.Where(x => x.Bounds.Contains(p)))
            {
                yield return x;
            }

            if (this.HasSplit)
            {
                int index = this.DetermineRegion(p);
                foreach (T item in this.nodes[index].FindAtPoint(p))
                {
                    yield return item;
                }
            }
        }

        private void Split()
        {
            int subWidth = this.bounds.Width / 2;
            int subWidthRemainder = this.bounds.Width % 2;
            int subHeight = this.bounds.Height / 2;
            int subHeightRemainder = this.bounds.Height % 2;
            int x = this.bounds.X;
            int y = this.bounds.Y;

            this.nodes[TopLeftIndex] = new QuadTree<T>(new Rectangle(x, y, subWidth, subHeight));
            this.nodes[TopRightIndex] = new QuadTree<T>(new Rectangle(x + subWidth, y, subWidth + subWidthRemainder, subHeight));
            this.nodes[BottomLeftIndex] = new QuadTree<T>(new Rectangle(x, y + subHeight, subWidth, subHeight + subHeightRemainder));
            this.nodes[BottomRightIndex] = new QuadTree<T>(new Rectangle(x + subWidth, y + subHeight, subWidth + subWidthRemainder, subHeight + subHeightRemainder));
        }

        private void UnSplit()
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                this.nodes[i] = null;
            }
        }

        private int DetermineRegion(Rectangle r)
        {
            if (!this.bounds.IntersectsWith(r))
            {
                return -1;
            }

            int midX = this.bounds.X + (this.bounds.Width / 2);
            int midY = this.bounds.Y + (this.bounds.Height / 2);

            int region;

            // decide left or right
            if (r.Right - 1 < midX)
            {
                // decide top or bottom
                if (r.Bottom - 1 < midY)
                {
                    region = TopLeftIndex;
                }
                else if (r.Top >= midY)
                {
                    region = BottomLeftIndex;
                }
                else
                {
                    region = -1;
                }
            }
            else if (r.Left >= midX)
            {
                // decide top or bottom
                if (r.Bottom - 1 < midY)
                {
                    region = TopRightIndex;
                }
                else if (r.Top >= midY)
                {
                    region = BottomRightIndex;
                }
                else
                {
                    region = -1;
                }
            }
            else
            {
                region = -1;
            }

            return region;
        }

        private int DetermineRegion(Point p)
        {
            int midX = this.bounds.X + (this.bounds.Width / 2);
            int midY = this.bounds.Y + (this.bounds.Height / 2);

            int region;

            // decide left or right
            if (p.X < midX)
            {
                // decide top or bottom
                region = p.Y < midY ? TopLeftIndex : BottomLeftIndex;
            }
            else
            {
                // decide top or bottom
                region = p.Y < midY ? TopRightIndex : BottomRightIndex;
            }

            return region;
        }
    }
}
