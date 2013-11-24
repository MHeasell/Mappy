namespace Geometry
{
    public struct Ray3D
    {
        public Ray3D(Vector3D origin, Vector3D direction) : this()
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        public Vector3D Origin { get; set; }

        public Vector3D Direction { get; set; }

        public Vector3D PointAt(double t)
        {
            return this.Origin + (this.Direction * t);
        }
    }
}
