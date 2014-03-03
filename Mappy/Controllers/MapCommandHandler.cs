namespace Mappy.Controllers
{
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.Models;

    public class MapCommandHandler
    {
        private readonly IMapSelectionModel model;

        private bool mouseDown;

        private Point lastMousePos;

        public MapCommandHandler(IMapSelectionModel model)
        {
            this.model = model;
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

                if (!this.model.HasSelection)
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
    }
}
