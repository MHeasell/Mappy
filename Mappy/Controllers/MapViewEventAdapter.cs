namespace Mappy.Controllers
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Mappy.UI.Controls;

    public class MapViewEventAdapter
    {
        private readonly IMapCommandHandler commandHandler;

        private readonly ImageLayerView view;

        public MapViewEventAdapter(ImageLayerView view, IMapCommandHandler handler)
        {
            this.view = view;
            this.commandHandler = handler;

            this.RegisterWithView();
        }

        private void RegisterWithView()
        {
            this.view.MouseDown += this.ViewMouseDown;
            this.view.MouseMove += this.ViewMouseMove;
            this.view.MouseUp += this.ViewMouseUp;
            this.view.KeyDown += this.ViewKeyDown;

            this.view.DragEnter += this.ViewDragEnter;
            this.view.DragDrop += this.ViewDragDrop;

            this.view.LostFocus += this.ViewLostFocus;
        }

        private void ViewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(StartPositionDragData)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ViewDragDrop(object sender, DragEventArgs e)
        {
            Point pos = this.view.PointToClient(new Point(e.X, e.Y));
            pos = this.view.ToVirtualPoint(pos);

            this.commandHandler.DragDrop(e.Data, pos.X, pos.Y);
        }

        private void ViewKeyDown(object sender, KeyEventArgs e)
        {
            this.commandHandler.KeyDown(e.KeyCode);
        }

        private void ViewMouseDown(object sender, MouseEventArgs e)
        {
            var virtualLoc = this.view.ToVirtualPoint(e.Location);
            this.commandHandler.MouseDown(virtualLoc.X, virtualLoc.Y);
        }

        private void ViewMouseMove(object sender, MouseEventArgs e)
        {
            var virtualLoc = this.view.ToVirtualPoint(e.Location);
            this.commandHandler.MouseMove(virtualLoc.X, virtualLoc.Y);
        }

        private void ViewMouseUp(object sender, MouseEventArgs e)
        {
            var virtualLoc = this.view.ToVirtualPoint(e.Location);
            this.commandHandler.MouseUp(virtualLoc.X, virtualLoc.Y);
        }

        private void ViewLostFocus(object sender, EventArgs eventArgs)
        {
            this.commandHandler.LostFocus();
        }
    }
}
