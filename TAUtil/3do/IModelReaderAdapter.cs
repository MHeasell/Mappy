namespace TAUtil._3do
{
    public interface IModelReaderAdapter
    {
        void CreateChild(string name, Vector position);

        void BackToParent();

        void AddVertex(Vector v);

        void AddPrimitive(int color, string texture, int[] vertexIndices, bool isSelectionPrimitive);
    }
}
