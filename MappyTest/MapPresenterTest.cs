namespace MappyTest
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Collections;
    using Mappy.Controllers.Tags;
    using Mappy.Data;
    using Mappy.Models;
    using Mappy.Presentation;
    using Mappy.UI.Controls;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class MapPresenterTest
    {
        private MapPresenter presenter;

        private Mock<IMainModel> model;

        private Mock<IMapView> view;

        [TestInitialize]
        public void SetUp()
        {
            this.model = new Mock<IMainModel>(MockBehavior.Strict);
            this.model.SetupGet(x => x.MapOpen).Returns(true);
            this.model.SetupGet(x => x.MapWidth).Returns(32);
            this.model.SetupGet(x => x.MapHeight).Returns(32);
            this.model.SetupGet(x => x.HeightmapVisible).Returns(false);
            this.model.SetupGet(x => x.BaseTile).Returns(new MapTile(32, 32));
            this.model.SetupGet(x => x.FloatingTiles).Returns(new List<Positioned<IMapTile>>());
            this.model.SetupGet(x => x.Features).Returns(new SparseGrid<Feature>(64, 64));
            this.model.Setup(x => x.GetStartPosition(It.IsAny<int>())).Returns((Point?)null);

            this.model.SetupGet(x => x.GridVisible).Returns(false);
            this.model.SetupGet(x => x.GridColor).Returns(Color.Black);
            this.model.SetupGet(x => x.GridSize).Returns(new Size(32, 32));
            this.model.SetupGet(x => x.SeaLevel).Returns(0);

            this.view = new Mock<IMapView>(MockBehavior.Strict);
            this.view.SetupGet(x => x.Items).Returns(new List<ImageLayerCollection.Item>());
            this.view.SetupProperty(x => x.CanvasSize);
            this.view.SetupProperty(x => x.GridVisible);
            this.view.SetupProperty(x => x.GridColor);
            this.view.SetupProperty(x => x.GridSize);

            this.presenter = new MapPresenter(this.view.Object, this.model.Object);

            this.model.ResetCalls();
            this.view.ResetCalls();
        }

        [TestClass]
        public class SelectItems : MapPresenterTest
        {
            /// <summary>
            /// Tests that, when the mouse is clicked on a tile
            /// that is not selected,
            /// we attempt to select the tile.
            /// </summary>
            [TestMethod]
            public void TestSelectTile()
            {
                var item = new ImageLayerCollection.Item(2, 4, 5, null);
                item.Tag = new SectionTag(3);

                this.view.Setup(x => x.IsInSelection(2, 4)).Returns(false);
                this.view.Setup(x => x.HitTest(2, 4)).Returns(item);

                this.model.Setup(x => x.SelectTile(3));

                this.presenter.MouseDown(2, 4);

                this.model.Verify(x => x.SelectTile(3), Times.Once);
            }

            /// <summary>
            /// Similar to previous test but with different coordinates.
            /// </summary>
            [TestMethod]
            public void TestSelectTile2()
            {
                var item = new ImageLayerCollection.Item(3, 8, 7, null);
                item.Tag = new SectionTag(2);

                this.view.Setup(x => x.IsInSelection(3, 8)).Returns(false);
                this.view.Setup(x => x.HitTest(3, 8)).Returns(item);

                this.model.Setup(x => x.SelectTile(2));

                this.presenter.MouseDown(3, 8);

                this.model.Verify(x => x.SelectTile(2), Times.Once);
            }

            /// <summary>
            /// Tests that, when the mouse is clicked on a feature
            /// that is not selected,
            /// we attempt to select the feature.
            /// </summary>
            [TestMethod]
            public void TestSelectFeature()
            {
                var item = new ImageLayerCollection.Item(2, 4, 5, null);
                item.Tag = new FeatureTag(new GridCoordinates(6, 7));

                this.view.Setup(x => x.IsInSelection(2, 4)).Returns(false);
                this.view.Setup(x => x.HitTest(2, 4)).Returns(item);

                this.model.Setup(x => x.SelectFeature(new GridCoordinates(6, 7)));

                this.presenter.MouseDown(2, 4);

                this.model.Verify(x => x.SelectFeature(new GridCoordinates(6, 7)), Times.Once);
            }

            /// <summary>
            /// Tests that, when the mouse is clicked on a start position
            /// that is not selected,
            /// we attempt to select the start position.
            /// </summary>
            [TestMethod]
            public void TestSelectStartPosition()
            {
                var item = new ImageLayerCollection.Item(2, 4, 5, null);
                item.Tag = new StartPositionTag(2);

                this.view.Setup(x => x.IsInSelection(2, 4)).Returns(false);
                this.view.Setup(x => x.HitTest(2, 4)).Returns(item);

                this.model.Setup(x => x.SelectStartPosition(2));

                this.presenter.MouseDown(2, 4);

                this.model.Verify(x => x.SelectStartPosition(2), Times.Once);
            }

            /// <summary>
            /// Tests that, when the mouse is clicked in an empty area,
            /// the current selection is cleared.
            /// </summary>
            [TestMethod]
            public void TestClearSelection()
            {
                this.view.Setup(x => x.IsInSelection(2, 4)).Returns(false);
                this.view.Setup(x => x.HitTest(2, 4)).Returns((ImageLayerCollection.Item)null);

                this.model.Setup(x => x.ClearSelection());
                this.model.Setup(x => x.StartBandbox(It.IsAny<int>(), It.IsAny<int>()));

                this.presenter.MouseDown(2, 4);

                this.model.Verify(x => x.ClearSelection(), Times.Once);
            }
        }

        [TestClass]
        public class DragTranslate : MapPresenterTest
        {
            /// <summary>
            /// Tests that when the mouse is clicked down on a selection
            /// and dragged, we translate that selection.
            /// </summary>
            [TestMethod]
            public void TestDrag()
            {
                this.view.Setup(x => x.IsInSelection(2, 4)).Returns(true);
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
                this.view.Setup(x => x.IsInSelection(2, 4)).Returns(true);
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

                this.view.Setup(x => x.IsInSelection(5, 3)).Returns(true);

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
                this.view.Setup(x => x.IsInSelection(6, 7)).Returns(true);
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
                this.view.Setup(x => x.IsInSelection(6, 7)).Returns(true);
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
        public class BandBoxSelection : MapPresenterTest
        {
            /// <summary>
            /// Tests that, when the mouse is clicked down over empty space,
            /// a bandbox is started.
            /// </summary>
            [TestMethod]
            public void TestBandboxSelect()
            {
                this.view.Setup(x => x.IsInSelection(1, 1)).Returns(false);
                this.view.Setup(x => x.HitTest(1, 1)).Returns((ImageLayerCollection.Item)null);

                this.model.Setup(x => x.ClearSelection());
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
                this.view.Setup(x => x.IsInSelection(5, 3)).Returns(false);
                this.view.Setup(x => x.HitTest(5, 3)).Returns((ImageLayerCollection.Item)null);

                this.model.Setup(x => x.ClearSelection());
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
                this.view.Setup(x => x.IsInSelection(5, 3)).Returns(false);
                this.view.Setup(x => x.HitTest(5, 3)).Returns((ImageLayerCollection.Item)null);

                this.model.Setup(x => x.ClearSelection());
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
                this.view.Setup(x => x.IsInSelection(5, 3)).Returns(false);
                this.view.Setup(x => x.HitTest(5, 3)).Returns((ImageLayerCollection.Item)null);

                this.model.Setup(x => x.ClearSelection());
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
                this.model.Setup(x => x.ClearSelection());
                this.model.Setup(x => x.StartBandbox(It.IsAny<int>(), It.IsAny<int>()));
                this.model.Setup(x => x.GrowBandbox(It.IsAny<int>(), It.IsAny<int>()));
                this.model.Setup(x => x.CommitBandbox());
                this.view.Setup(x => x.IsInSelection(1, 1)).Returns(false);
                this.view.Setup(x => x.HitTest(1, 1)).Returns((ImageLayerCollection.Item)null);

                this.presenter.MouseDown(1, 1);
                this.presenter.MouseUp(1, 1);

                this.view.Setup(x => x.IsInSelection(1, 1)).Returns(true);
                this.model.Setup(x => x.TranslateSelection(1, 2));

                this.presenter.MouseDown(1, 1);
                this.presenter.MouseMove(2, 3);

                this.model.Verify(x => x.TranslateSelection(1, 2), Times.Once);
            }
        }

        [TestClass]
        public class Delete : MapPresenterTest
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