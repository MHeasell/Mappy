namespace MappyTest
{
    using System;

    using Mappy;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MaybeTest
    {
        [TestMethod]
        public void TestFromValue()
        {
            var i = Maybe.From("asdf");
            i.Do(
                some: x => Assert.AreEqual("asdf", x),
                none: Assert.Fail);
        }

        [TestMethod]
        public void TestFromNull()
        {
            var i = Maybe.From((string)null);

            Assert.IsTrue(i.IsNone);
            Assert.IsFalse(i.IsSome);

            i.IfSome(_ => Assert.Fail());

            var passed = false;
            i.IfNone(() => passed = true);
            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void TestSome()
        {
            var i = Maybe.Some(2);
            Assert.IsTrue(i.IsSome);
            Assert.IsFalse(i.IsNone);

            i.Do(
                some: x => Assert.AreEqual(2, x),
                none: Assert.Fail);
        }

        [TestMethod]
        public void TestSomeThrowsOnNull()
        {
            try
            {
                Maybe.Some((string)null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                // we passed
            }
        }

        [TestMethod]
        public void TestMap()
        {
            var i = Maybe.Some(2);
            var j = i.Map(x => x.ToString());
            j.Do(
                some: x => Assert.AreEqual("2", x),
                none: Assert.Fail);
        }

        [TestMethod]
        public void TestMapNull()
        {
            var i = Maybe.None<int>();
            var j = i.Map(x => x.ToString());

            var passed = false;
            j.Do(
                some: x => Assert.Fail(),
                none: () => passed = true);
            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void TestWhereFalse()
        {
            var i = Maybe.Some(1);
            var j = i.Where(_ => false);

            Assert.IsTrue(j.IsNone);
        }

        [TestMethod]
        public void TestWhereTrue()
        {
            var i = Maybe.Some(1);
            var j = i.Where(_ => true);

            j.Do(
                some: x => Assert.AreEqual(1, x),
                none: Assert.Fail);
        }

        [TestMethod]
        public void TestWhereNull()
        {
            var i = Maybe.None<int>();
            var j = i.Where(_ => true);

            Assert.IsTrue(j.IsNone);
        }

        [TestMethod]
        public void TestOrValueNone()
        {
            var i = Maybe.None<int>();
            var j = i.Or(23);
            Assert.AreEqual(23, j);
        }

        [TestMethod]
        public void TestOrValueSome()
        {
            var i = Maybe.Some(21);
            var j = i.Or(23);
            Assert.AreEqual(21, j);
        }
    }
}
