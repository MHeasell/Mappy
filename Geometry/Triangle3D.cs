namespace Geometry
{
    public struct Triangle3D
    {
        public Triangle3D(Vector3D a, Vector3D b, Vector3D c) : this()
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public Vector3D A { get; set; }

        public Vector3D B { get; set; }

        public Vector3D C { get; set; }

        public Vector3D Normal()
        {
            return Vector3D.Cross(this.C - this.A, this.B - this.A).Normalized();
        }

        public Plane3D Plane()
        {
            return new Plane3D(this.A, this.Normal());
        }

        public Vector3D ToBarycentric(Vector3D point)
        {
            var v0 = this.C - this.A;
            var v1 = this.B - this.A;
            var v2 = point - this.A;

            var u = (
                (Vector3D.Dot(v1, v1) * Vector3D.Dot(v2, v0))
                - (Vector3D.Dot(v1, v0) * Vector3D.Dot(v2, v1)))
                /
                ((Vector3D.Dot(v0, v0) * Vector3D.Dot(v1, v1))
                - (Vector3D.Dot(v0, v1) * Vector3D.Dot(v1, v0)));

            var v = (
                (Vector3D.Dot(v0, v0) * Vector3D.Dot(v2, v1))
                - (Vector3D.Dot(v0, v1) * Vector3D.Dot(v2, v0)))
                /
                ((Vector3D.Dot(v0, v0) * Vector3D.Dot(v1, v1))
                - (Vector3D.Dot(v0, v1) * Vector3D.Dot(v1, v0)));

            return new Vector3D(u, v, 1.0 - u - v);
        }

        public double Intersect(Ray3D ray)
        {
            var d = this.Plane().Intersect(ray);
            var bary = this.ToBarycentric(ray.PointAt(d));

            if (bary.X < 0.0 || bary.Y < 0.0 || bary.Z < 0.0)
            {
                return double.PositiveInfinity;
            }

            return d;
        }
    }
}
