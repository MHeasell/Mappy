namespace MappyTest
{
    using System.Drawing;
    using System.Linq;

    using Mappy.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QuadTreeTest
    {
        [TestMethod]
        public void TestInsert()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            Assert.AreEqual(0, tree.Count);

            TreeItem i = new TreeItem(new Rectangle(0, 0, 3, 3));

            tree.Add(i);

            Assert.IsTrue(tree.Contains(i));
            Assert.AreEqual(1, tree.Count);
        }

        [TestMethod]
        public void TestContains()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            TreeItem i = new TreeItem(new Rectangle(0, 0, 3, 3));
            TreeItem j = new TreeItem(new Rectangle(20, 20, 40, 40));
            TreeItem k = new TreeItem(new Rectangle(15, 2, 4, 6));

            tree.Add(i);
            tree.Add(j);

            Assert.IsTrue(tree.Contains(i));
            Assert.IsTrue(tree.Contains(j));
            Assert.IsFalse(tree.Contains(k));
        }

        [TestMethod]
        public void TestRemove()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            TreeItem i = new TreeItem(new Rectangle(0, 0, 3, 3));
            TreeItem j = new TreeItem(new Rectangle(20, 20, 40, 40));

            tree.Add(i);

            Assert.AreEqual(1, tree.Count);

            tree.Add(j);

            Assert.AreEqual(2, tree.Count);

            bool success = tree.Remove(j);

            Assert.IsTrue(success);
            Assert.AreEqual(1, tree.Count);

            Assert.IsTrue(tree.Contains(i));
            Assert.IsFalse(tree.Contains(j));
        }

        [TestMethod]
        public void TestFindAtPoint()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            TreeItem i = new TreeItem(new Rectangle(3, 3, 7, 8));

            tree.Add(i);

            var result = tree.FindAtPoint(new Point(4, 4)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(i, result[0]);
        }

        [TestMethod]
        public void TestFindAtPoint2()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            TreeItem i = new TreeItem(new Rectangle(3, 3, 7, 8));

            tree.Add(i);

            var result = tree.FindAtPoint(new Point(2, 20)).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestFindInAreaSmaller()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            TreeItem i = new TreeItem(new Rectangle(3, 3, 12, 12));

            tree.Add(i);

            var result = tree.FindInArea(new Rectangle(4, 4, 1, 1)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(i, result[0]);
        }

        [TestMethod]
        public void TestFindInAreaBigger()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            TreeItem i = new TreeItem(new Rectangle(3, 3, 12, 12));

            tree.Add(i);

            var result = tree.FindInArea(new Rectangle(2, 2, 45, 45)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(i, result[0]);
        }

        [TestMethod]
        public void TestFindInAreaEdgeHit()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            TreeItem i = new TreeItem(new Rectangle(5, 5, 10, 10));

            tree.Add(i);

            var result = tree.FindInArea(new Rectangle(10, 14, 3, 3)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(i, result[0]);
        }

        [TestMethod]
        public void TestFindInAreaEdgeMiss()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            TreeItem i = new TreeItem(new Rectangle(5, 5, 10, 10));

            tree.Add(i);

            var result = tree.FindInArea(new Rectangle(10, 15, 3, 3)).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestFindInAreaLots()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 10, 10), 2);

            TreeItem i = new TreeItem(new Rectangle(2, 2, 2, 2));
            TreeItem j = new TreeItem(new Rectangle(6, 6, 3, 2));
            TreeItem k = new TreeItem(new Rectangle(1, 6, 2, 2));
            TreeItem l = new TreeItem(new Rectangle(7, 0, 2, 2));
            TreeItem m = new TreeItem(new Rectangle(0, 0, 4, 4));
            TreeItem n = new TreeItem(new Rectangle(4, 3, 6, 6));

            tree.Add(i);
            tree.Add(j);
            tree.Add(k);
            tree.Add(l);
            tree.Add(m);
            tree.Add(n);

            var result = tree.FindInArea(new Rectangle(1, 7, 8, 3)).ToList();

            Assert.IsFalse(result.Contains(i));
            Assert.IsTrue(result.Contains(j));
            Assert.IsTrue(result.Contains(k));
            Assert.IsFalse(result.Contains(l));
            Assert.IsFalse(result.Contains(m));
            Assert.IsTrue(result.Contains(n));
        }

        [TestMethod]
        public void TestRemoveBug()
        {
            QuadTree<TreeItem> tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 10, 10), 1);

            TreeItem i = new TreeItem(new Rectangle(0, 0, 3, 3));
            TreeItem j = new TreeItem(new Rectangle(0, 0, 2, 2));
            
            tree.Add(i);
            tree.Add(j);

            bool success = tree.Remove(j);

            Assert.IsTrue(success);
            Assert.AreEqual(1, tree.Count);
            Assert.IsTrue(tree.Contains(i));
            Assert.IsFalse(tree.Contains(j));
        }
    }
}
