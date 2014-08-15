namespace TAUtil._3do
{
    using System.IO;
    using System.Text;

    public abstract class Abstract3doReader
    {
        public void Read(Stream s)
        {
            this.Read(new BinaryReader(s));
        }

        public void Read(BinaryReader b)
        {
            var header = new ObjectHeader();

            ObjectHeader.Read(b, ref header);

            b.BaseStream.Seek(header.PtrObjectName, SeekOrigin.Begin);
            var name = this.ReadString(b);

            this.CreateChild(name, header.Position);

            b.BaseStream.Seek(header.PtrVertexArray, SeekOrigin.Begin);

            for (int i = 0; i < header.VertexCount; i++)
            {
                var v = new Vector();
                Vector.Read(b, ref v);
                this.AddVertex(v);
            }

            for (int i = 0; i < header.PrimitiveCount; i++)
            {
                int offset = i * PrimitiveHeader.Length;
                b.BaseStream.Seek(
                    header.PtrPrimitiveArray + offset,
                    SeekOrigin.Begin);
                this.ReadPrimitive(b);
            }

            if (header.PtrChildObject != 0)
            {
                b.BaseStream.Seek(header.PtrChildObject, SeekOrigin.Begin);
                this.Read(b);
            }

            this.BackToParent();

            if (header.PtrSiblingObject != 0)
            {
                b.BaseStream.Seek(header.PtrSiblingObject, SeekOrigin.Begin);
                this.Read(b);
            }
        }

        protected abstract void CreateChild(string name, Vector position);

        protected abstract void BackToParent();

        protected abstract void AddVertex(Vector v);

        protected abstract void AddPrimitive(int color, string texture, int[] vertexIndices);

        private void ReadPrimitive(BinaryReader b)
        {
            var header = new PrimitiveHeader();
            PrimitiveHeader.Read(b, ref header);

            var vertices = new int[header.VertexCount];

            b.BaseStream.Seek(header.PtrVertexIndexArray, SeekOrigin.Begin);

            for (int i = 0; i < header.VertexCount; i++)
            {
                vertices[i] = b.ReadUInt16();
            }

            b.BaseStream.Seek(header.PtrTextureName, SeekOrigin.Begin);

            string texture = this.ReadString(b);

            this.AddPrimitive(header.ColorIndex, texture, vertices);
        }

        private string ReadString(BinaryReader b)
        {
            StringBuilder nameBuilder = new StringBuilder();
            byte c;
            while ((c = b.ReadByte()) != 0)
            {
                nameBuilder.Append((char)c);
            }

            return nameBuilder.ToString();
        }
    }
}
