namespace Mappy.Models.Session
{
    public interface ISelectionCommandHandler
    {
        void SelectAtPoint(int x, int y);

        void ClearSelection();

        void DeleteSelection();

        void TranslateSelection(int x, int y);

        void FlushTranslation();

        void DragDropFeature(string name, int x, int y);

        void DragDropTile(int id, int x, int y);

        void DragDropStartPosition(int index, int x, int y);
    }
}
