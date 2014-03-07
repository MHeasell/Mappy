namespace MappyTest
{
    using System.Windows.Forms;

    using Mappy.Models.Session;
    using Mappy.Presentation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class MapCommandHandlerTest
    {
        private MapCommandHandler presenter;

        private Mock<ISelectionCommandHandler> model;

        [TestInitialize]
        public void SetUp()
        {
            this.model = new Mock<ISelectionCommandHandler>(MockBehavior.Strict);
            this.presenter = new MapCommandHandler(this.model.Object);
        }

        [TestClass]
        public class SelectAtPoint : MapCommandHandlerTest
        {
            /// <summary>
            /// Tests that, when the mouse is clicked on something not selected,
            /// we attempt to select at that point.
            /// </summary>
            [TestMethod]
            public void TestSelectAtPoint()
            {
                this.model.Setup(x => x.IsInSelection(2, 4)).Returns(false);
                this.model.Setup(x => x.SelectAtPoint(2, 4)).Returns(true);

                this.presenter.MouseDown(2, 4);

                this.model.Verify(x => x.SelectAtPoint(2, 4), Times.Once);
            }

            /// <summary>
            /// Similar to previous test but with different coordinates.
            /// </summary>
            [TestMethod]
            public void TestSelectAtPoint2()
            {
                this.model.Setup(x => x.IsInSelection(3, 8)).Returns(false);
                this.model.Setup(x => x.SelectAtPoint(3, 8)).Returns(true);

                this.presenter.MouseDown(3, 8);

                this.model.Verify(x => x.SelectAtPoint(3, 8), Times.Once);
            }
        }

        [TestClass]
        public class DragTranslate : MapCommandHandlerTest
        {
            /// <summary>
            /// Tests that when the mouse is clicked down on a selection
            /// and dragged, we translate that selection.
            /// </summary>
            [TestMethod]
            public void TestDrag()
            {
                this.model.Setup(x => x.IsInSelection(2, 4)).Returns(true);
                this.model.Setup(x => x.TranslateSelection(2, 2));

                this.presenter.MouseDown(2, 4);

                this.presenter.MouseMove(4, 6);

                this.model.Verify(x => x.TranslateSelection(2, 2), Times.Once);
            }

            /// <summary>
            /// Similar to previous test, but with different coordinates.
            /// </summary>
            [TestMethod]
            public void TestDrag2()
            {
                this.model.Setup(x => x.IsInSelection(2, 4)).Returns(true);
                this.model.Setup(x => x.TranslateSelection(3, 7));

                this.presenter.MouseDown(2, 4);

                this.presenter.MouseMove(5, 11);

                this.model.Verify(x => x.TranslateSelection(3, 7), Times.Once);
            }

            /// <summary>
            /// Tests that, when the mouse is moved without clicking down,
            /// nothing happens.
            /// </summary>
            [TestMethod]
            public void TestDragNoMouseDown()
            {
                this.presenter.MouseMove(3, 7);

                this.model.Verify(x => x.TranslateSelection(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            }

            /// <summary>
            /// Tests that, after the mouse button is released,
            /// dragging does not occur.
            /// </summary>
            [TestMethod]
            public void TestDragAfterMouseUp()
            {
                // don't care if this is called
                this.model.Setup(x => x.FlushTranslation());

                this.model.Setup(x => x.IsInSelection(5, 3)).Returns(true);

                this.presenter.MouseDown(5, 3);

                this.presenter.MouseUp(5, 3);

                this.presenter.MouseMove(3, 7);

                this.model.Verify(x => x.TranslateSelection(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            }

            /// <summary>
            /// Tests that translation is flushed after releasing the button.
            /// </summary>
            [TestMethod]
            public void TestDragFlush()
            {
                this.model.Setup(x => x.IsInSelection(6, 7)).Returns(true);
                this.model.Setup(x => x.FlushTranslation());

                // don't care about calls to this
                this.model.Setup(x => x.TranslateSelection(It.IsAny<int>(), It.IsAny<int>()));

                this.presenter.MouseDown(6, 7);

                this.presenter.MouseMove(1, 1);

                this.presenter.MouseUp(7, 8);

                this.model.Verify(x => x.FlushTranslation(), Times.Once);
            }

            /// <summary>
            /// Tests that the translation is only flushed once,
            /// upon the release of the button.
            /// </summary>
            [TestMethod]
            public void TestDragFlushMultiple()
            {
                this.model.Setup(x => x.IsInSelection(6, 7)).Returns(true);
                this.model.Setup(x => x.FlushTranslation());

                // don't care about calls to this
                this.model.Setup(x => x.TranslateSelection(It.IsAny<int>(), It.IsAny<int>()));

                this.presenter.MouseDown(6, 7);

                this.presenter.MouseMove(1, 1);
                this.presenter.MouseMove(1, 1);

                this.presenter.MouseUp(8, 9);

                this.model.Verify(x => x.FlushTranslation(), Times.Once);
            }
        }

        [TestClass]
        public class BandBoxSelection : MapCommandHandlerTest
        {
            /// <summary>
            /// Tests that, when the mouse is clicked down over empty space,
            /// a bandbox is started.
            /// </summary>
            [TestMethod]
            public void TestBandboxSelect()
            {
                this.model.Setup(x => x.IsInSelection(1, 1)).Returns(false);
                this.model.Setup(x => x.SelectAtPoint(1, 1)).Returns(false);

                this.model.Setup(x => x.StartBandbox(1, 1));

                this.presenter.MouseDown(1, 1);

                this.model.Verify(x => x.StartBandbox(1, 1));
            }

            /// <summary>
            /// Similar to previous test, but with different coordinates.
            /// </summary>
            [TestMethod]
            public void TestBandboxSelect2()
            {
                this.model.Setup(x => x.IsInSelection(5, 3)).Returns(false);
                this.model.Setup(x => x.SelectAtPoint(5, 3)).Returns(false);

                this.model.Setup(x => x.StartBandbox(5, 3));

                this.presenter.MouseDown(5, 3);

                this.model.Verify(x => x.StartBandbox(5, 3));
            }

            /// <summary>
            /// Tests that, when an empty space is clicked and dragged,
            /// the bandbox is grown.
            /// </summary>
            [TestMethod]
            public void TestBandboxDrag()
            {
                this.model.Setup(x => x.IsInSelection(5, 3)).Returns(false);
                this.model.Setup(x => x.SelectAtPoint(5, 3)).Returns(false);

                this.model.Setup(x => x.StartBandbox(It.IsAny<int>(), It.IsAny<int>()));
                this.model.Setup(x => x.GrowBandbox(2, 2));

                this.presenter.MouseDown(5, 3);

                this.presenter.MouseMove(7, 5);

                this.model.Verify(x => x.GrowBandbox(2, 2));
            }

            /// <summary>
            /// Tests that, when an empty space is clicked, dragged
            /// and the button released, the bandbox is committed.
            /// </summary>
            [TestMethod]
            public void TestBandboxCommitOnRelease()
            {
                this.model.Setup(x => x.IsInSelection(5, 3)).Returns(false);
                this.model.Setup(x => x.SelectAtPoint(5, 3)).Returns(false);

                this.model.Setup(x => x.StartBandbox(It.IsAny<int>(), It.IsAny<int>()));
                this.model.Setup(x => x.GrowBandbox(It.IsAny<int>(), It.IsAny<int>()));
                this.model.Setup(x => x.CommitBandbox());

                this.presenter.MouseDown(5, 3);

                this.presenter.MouseMove(2, 2);

                this.presenter.MouseUp(7, 5);

                this.model.Verify(x => x.CommitBandbox());
            }

            /// <summary>
            /// Tests that clicking an item and dragging it
            /// still works as expected after clicking somewhere empty
            /// and starting a bandbox.
            /// </summary>
            [TestMethod]
            public void TestSelectAfterEmptyClick()
            {
                this.model.Setup(x => x.StartBandbox(It.IsAny<int>(), It.IsAny<int>()));
                this.model.Setup(x => x.GrowBandbox(It.IsAny<int>(), It.IsAny<int>()));
                this.model.Setup(x => x.CommitBandbox());
                this.model.Setup(x => x.IsInSelection(1, 1)).Returns(false);
                this.model.Setup(x => x.SelectAtPoint(1, 1)).Returns(false);

                this.presenter.MouseDown(1, 1);
                this.presenter.MouseUp(1, 1);

                this.model.Setup(x => x.SelectAtPoint(1, 1)).Returns(true);
                this.model.Setup(x => x.TranslateSelection(1, 2));

                this.presenter.MouseDown(1, 1);
                this.presenter.MouseMove(2, 3);

                this.model.Verify(x => x.TranslateSelection(1, 2), Times.Once);
            }
        }

        [TestClass]
        public class Delete : MapCommandHandlerTest
        {
            /// <summary>
            /// Tests that when the delete key is pressed,
            /// the current selection is deleted.
            /// </summary>
            [TestMethod]
            public void TestDelete()
            {
                this.model.Setup(x => x.DeleteSelection());

                this.presenter.KeyDown(Keys.Delete);

                this.model.Verify(x => x.DeleteSelection(), Times.Once);
            }

            /// <summary>
            /// Tests that nothing happens when a different key is pressed.
            /// </summary>
            [TestMethod]
            public void TestOtherKeyNoDelete()
            {
                this.presenter.KeyDown(Keys.D8);

                this.model.Verify(x => x.DeleteSelection(), Times.Never);
            }

            /// <summary>
            /// Similar to previous test, but with a different key.
            /// </summary>
            [TestMethod]
            public void TestOtherKeyNoDelete2()
            {
                this.presenter.KeyDown(Keys.B);

                this.model.Verify(x => x.DeleteSelection(), Times.Never);
            }
        }
    }
}