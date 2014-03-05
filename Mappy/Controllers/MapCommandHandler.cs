namespace Mappy.Controllers
{
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Models;
    using Mappy.UI.Controls;

    public class MapCommandHandler : IMapCommandHandler
    {
        private readonly ISelectionCommandModel model;

        private bool mouseDown;

        private Point lastMousePos;

        public MapCommandHandler(ISelectionCommandModel model)
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
            this.model.SelectAtPoint(virtualX, virtualY);
        }

        public void MouseMove(int virtualX, int virtualY)
        {
            try
            {
                if (!this.mouseDown)
                {
                    return;
                }

                this.model.TranslateSelection(
                    virtualX - this.lastMousePos.X,
                    virtualY - this.lastMousePos.Y);
            }
            finally
            {
                this.lastMousePos = new Point(virtualX, virtualY);
            }
        }

        public void MouseUp(int virtualX, int virtualY)
        {
            this.model.FlushTranslation();
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
