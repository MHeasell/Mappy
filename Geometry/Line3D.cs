namespace Geometry
{
    public struct Line3D
    {
        public Line3D(Vector3D start, Vector3D end) : this()
        {
            this.Start = start;
            this.End = end;
        }

        public Vector3D Start { get; set; }

        public Vector3D End { get; set; }

        public Ray3D ToRay()
        {
            return new Ray3D(this.Start, this.End - this.Start);
        }

        public Vector3D PointAt(double d)
        {
            return this.Start + ((this.End - this.Start) * d);
        }
    }
}
