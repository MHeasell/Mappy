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
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            Assert.AreEqual(0, tree.Count);

            var i = new TreeItem(new Rectangle(0, 0, 3, 3));

            tree.Add(i);

            Assert.IsTrue(tree.Contains(i));
            Assert.AreEqual(1, tree.Count);
        }

        [TestMethod]
        public void TestContains()
        {
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            var i = new TreeItem(new Rectangle(0, 0, 3, 3));
            var j = new TreeItem(new Rectangle(20, 20, 40, 40));
            var k = new TreeItem(new Rectangle(15, 2, 4, 6));

            tree.Add(i);
            tree.Add(j);

            Assert.IsTrue(tree.Contains(i));
            Assert.IsTrue(tree.Contains(j));
            Assert.IsFalse(tree.Contains(k));
        }

        [TestMethod]
        public void TestRemove()
        {
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            var i = new TreeItem(new Rectangle(0, 0, 3, 3));
            var j = new TreeItem(new Rectangle(20, 20, 40, 40));

            tree.Add(i);

            Assert.AreEqual(1, tree.Count);

            tree.Add(j);

            Assert.AreEqual(2, tree.Count);

            var success = tree.Remove(j);

            Assert.IsTrue(success);
            Assert.AreEqual(1, tree.Count);

            Assert.IsTrue(tree.Contains(i));
            Assert.IsFalse(tree.Contains(j));
        }

        [TestMethod]
        public void TestFindAtPoint()
        {
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            var i = new TreeItem(new Rectangle(3, 3, 7, 8));

            tree.Add(i);

            var result = tree.FindAtPoint(new Point(4, 4)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(i, result[0]);
        }

        [TestMethod]
        public void TestFindAtPoint2()
        {
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            var i = new TreeItem(new Rectangle(3, 3, 7, 8));

            tree.Add(i);

            var result = tree.FindAtPoint(new Point(2, 20)).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestFindInAreaSmaller()
        {
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            var i = new TreeItem(new Rectangle(3, 3, 12, 12));

            tree.Add(i);

            var result = tree.FindInArea(new Rectangle(4, 4, 1, 1)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(i, result[0]);
        }

        [TestMethod]
        public void TestFindInAreaBigger()
        {
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            var i = new TreeItem(new Rectangle(3, 3, 12, 12));

            tree.Add(i);

            var result = tree.FindInArea(new Rectangle(2, 2, 45, 45)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(i, result[0]);
        }

        [TestMethod]
        public void TestFindInAreaEdgeHit()
        {
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            var i = new TreeItem(new Rectangle(5, 5, 10, 10));

            tree.Add(i);

            var result = tree.FindInArea(new Rectangle(10, 14, 3, 3)).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(i, result[0]);
        }

        [TestMethod]
        public void TestFindInAreaEdgeMiss()
        {
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 50, 50));

            var i = new TreeItem(new Rectangle(5, 5, 10, 10));

            tree.Add(i);

            var result = tree.FindInArea(new Rectangle(10, 15, 3, 3)).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestFindInAreaLots()
        {
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 10, 10), 2);

            var i = new TreeItem(new Rectangle(2, 2, 2, 2));
            var j = new TreeItem(new Rectangle(6, 6, 3, 2));
            var k = new TreeItem(new Rectangle(1, 6, 2, 2));
            var l = new TreeItem(new Rectangle(7, 0, 2, 2));
            var m = new TreeItem(new Rectangle(0, 0, 4, 4));
            var n = new TreeItem(new Rectangle(4, 3, 6, 6));

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
            var tree = new QuadTree<TreeItem>(new Rectangle(0, 0, 10, 10), 1);

            var i = new TreeItem(new Rectangle(0, 0, 3, 3));
            var j = new TreeItem(new Rectangle(0, 0, 2, 2));
            
            tree.Add(i);
            tree.Add(j);

            var success = tree.Remove(j);

            Assert.IsTrue(success);
            Assert.AreEqual(1, tree.Count);
            Assert.IsTrue(tree.Contains(i));
            Assert.IsFalse(tree.Contains(j));
        }
    }
}
