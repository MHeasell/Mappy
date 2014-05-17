namespace Mappy.Presentation
{
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Models.Session;
    using Mappy.UI.Controls;

    public class MapCommandHandler : IMapCommandHandler
    {
        private readonly ISelectionModel model;

        private bool mouseDown;

        private Point lastMousePos;

        private bool bandboxMode;

        public MapCommandHandler(ISelectionModel model)
        {
            this.model = model;
        }

        public void DragDrop(IDataObject data, int virtualX, int virtualY)
        {
            if (data.GetDataPresent(typeof(StartPositionDragData)))
            {
                StartPositionDragData posData = (StartPositionDragData)data.GetData(typeof(StartPositionDragData));
                this.model.DragDropStartPosition(posData.PositionNumber, virtualX, virtualY);
            }
            else
            {
                string dataString = data.GetData(DataFormats.Text).ToString();
                int id;
                if (int.TryParse(dataString, out id))
                {
                    this.model.DragDropTile(id, virtualX, virtualY);
                }
                else
                {
                    this.model.DragDropFeature(dataString, virtualX, virtualY);
                }
            }
        }

        public void MouseDown(int virtualX, int virtualY)
        {
            this.mouseDown = true;
            this.lastMousePos = new Point(virtualX, virtualY);

            if (!this.model.IsInSelection(virtualX, virtualY))
            {
                if (!this.model.SelectAtPoint(virtualX, virtualY))
                {
                    this.model.StartBandbox(virtualX, virtualY);
                    this.bandboxMode = true;
                }
            }
        }

        public void MouseMove(int virtualX, int virtualY)
        {
            try
            {
                if (!this.mouseDown)
                {
                    return;
                }

                if (this.bandboxMode)
                {
                    this.model.GrowBandbox(
                        virtualX - this.lastMousePos.X,
                        virtualY - this.lastMousePos.Y);
                }
                else
                {
                    this.model.TranslateSelection(
                        virtualX - this.lastMousePos.X,
                        virtualY - this.lastMousePos.Y);
                }
            }
            finally
            {
                this.lastMousePos = new Point(virtualX, virtualY);
            }
        }

        public void MouseUp(int virtualX, int virtualY)
        {
            if (this.bandboxMode)
            {
                this.model.CommitBandbox();
                this.bandboxMode = false;
            }
            else
            {
                this.model.FlushTranslation();
            }

            this.mouseDown = false;
        }

        public void KeyDown(Keys key)
        {
            if (key == Keys.Delete)
            {
                this.model.DeleteSelection();
            }
        }

        public void LostFocus()
        {
            this.model.ClearSelection();
        }
    }
}
