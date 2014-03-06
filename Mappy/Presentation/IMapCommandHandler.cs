namespace Mappy.Presentation
{
    using System.Windows.Forms;

    public interface IMapCommandHandler
    {
        void DragDrop(IDataObject data, int virtualX, int virtualY);

        void MouseDown(int virtualX, int virtualY);

        void MouseMove(int virtualX, int virtualY);

        void MouseUp(int virtualX, int virtualY);

        void KeyDown(Keys key);

        void LostFocus();
    }
}