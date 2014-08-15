namespace TAUtil._3do
{
    using System.IO;

    public struct ObjectHeader
    {
        /// <summary>
        /// This is field is always one.
        /// </summary>
        public int Version;

        /// <summary>
        /// The number of vertices used by this object.
        /// A vertex is simply a 3D point.
        /// </summary>
        public int VertexCount;

        /// <summary>
        /// The number of primitives used by this object.
        /// A primitive is a simple 3D object
        /// like a point, line, triangle, or quad.
        /// </summary>
        public int PrimitiveCount;

        /// <summary>
        /// In the parent object, this is the offset to a primitive
        /// that will serve as the "selection" rectangle in TA.
        /// All Child and Sibling objects should have this value set to -1.
        /// </summary>
        public int PtrSelectionPrimitive;

        /// <summary>
        /// The position of the object relative to the parent object.
        /// The first object in a file doesn't have any parents.
        /// </summary>
        public Vector Position;

        /// <summary>
        /// The offset to the name of the object.
        /// The name of the object is stored as a null terminated string.
        /// </summary>
        public int PtrObjectName;

        /// <summary>
        /// This field appears to always be zero.
        /// Purpose is unknown.
        /// </summary>
        public int AlwaysZero;

        /// <summary>
        /// The offset to the array of vertices used by this object.
        /// The number of vertices in the array is given by VertexCount.
        /// </summary>
        public int PtrVertexArray;

        /// <summary>
        /// The offset to the array of primitives used by this object.
        /// The number of primitives is given by PrimitiveCount.
        /// </summary>
        public int PtrPrimitiveArray;

        /// <summary>
        /// The offset to the next sibling object.
        /// A sibling object is an object
        /// which shares the same parent as this object.
        /// If there is no next sibling, this field will be 0.
        /// </summary>
        public int PtrSiblingObject;

        /// <summary>
        /// The offset to the first child of this object.
        /// A child is an object which has the current object as its parent.
        /// If this object has no child, this field will be 0.
        /// </summary>
        public int PtrChildObject;

        public static void Read(Stream file, ref ObjectHeader header)
        {
            Read(new BinaryReader(file), ref header);
        }

        public static void Read(BinaryReader b, ref ObjectHeader header)
        {
            header.Version = b.ReadInt32();
            header.VertexCount = b.ReadInt32();
            header.PrimitiveCount = b.ReadInt32();
            header.PtrSelectionPrimitive = b.ReadInt32();
            
            header.Position = new Vector();
            Vector.Read(b, ref header.Position);

            header.PtrObjectName = b.ReadInt32();
            header.AlwaysZero = b.ReadInt32();
            header.PtrVertexArray = b.ReadInt32();
            header.PtrPrimitiveArray = b.ReadInt32();
            header.PtrSiblingObject = b.ReadInt32();
            header.PtrChildObject = b.ReadInt32();
        }
    }
}
