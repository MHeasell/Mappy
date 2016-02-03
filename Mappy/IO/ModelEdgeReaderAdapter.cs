namespace Mappy.IO
{
    using System.Collections.Generic;

    using Geometry;

    using TAUtil._3do;

    public class ModelEdgeReaderAdapter : IModelReaderAdapter
    {
        private readonly List<Vector3D> vertices = new List<Vector3D>();

        private readonly List<Line3D> edges = new List<Line3D>();

        private readonly Stack<Vector3D> positions = new Stack<Vector3D>();

        public List<Line3D> Edges => this.edges;

        public void CreateChild(string name, Vector position)
        {
            this.PushNewOffset(position);
            this.vertices.Clear();
        }

        public void BackToParent()
        {
            this.positions.Pop();
            this.vertices.Clear();
        }

        public void AddVertex(Vector v)
        {
            var basePos = this.positions.Peek();
            var vec = new Vector3D(v.X, v.Y, v.Z);
            this.vertices.Add(basePos + vec);
        }

        public void AddPrimitive(int color, string texture, int[] vertexIndices, bool isSelectionPrimitive)
        {
            if (vertexIndices.Length <= 1 || isSelectionPrimitive)
            {
                return;
            }

            for (int i = 1; i < vertexIndices.Length; i++)
            {
                var vec0 = this.vertices[vertexIndices[i - 1]];
                var vec1 = this.vertices[vertexIndices[i]];

                this.edges.Add(new Line3D(vec0, vec1));
            }

            var vecLast = this.vertices[vertexIndices[vertexIndices.Length - 1]];
            var vecFirst = this.vertices[vertexIndices[0]];

            this.edges.Add(new Line3D(vecLast, vecFirst));
        }

        private void PushNewOffset(Vector offset)
        {
            var offsetVec = new Vector3D(offset.X, offset.Y, offset.Z);

            Vector3D parentPosition;
            if (this.positions.Count == 0)
            {
                parentPosition = Vector3D.Zero;
            }
            else
            {
                parentPosition = this.positions.Peek();
            }

            var newPos = parentPosition + offsetVec;

            this.positions.Push(newPos);
        }
    }
}
