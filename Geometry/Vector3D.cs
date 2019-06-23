namespace Geometry
{
    using System;

    public struct Vector3D
    {
        public static readonly Vector3D ZAxis = new Vector3D(0.0, 0.0, 1.0);
        public static readonly Vector3D YAxis = new Vector3D(0.0, 1.0, 0.0);
        public static readonly Vector3D XAxis = new Vector3D(1.0, 0.0, 0.0);

        public static readonly Vector3D Zero = new Vector3D(0.0, 0.0, 0.0);

        public Vector3D(double x, double y, double z) : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator -(Vector3D v)
        {
            return new Vector3D(-v.X, -v.Y, -v.Z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3D operator *(Vector3D v, double scalar)
        {
            return new Vector3D(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Vector3D operator /(Vector3D v, double scalar)
        {
            return new Vector3D(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }

        public static double Dot(Vector3D a, Vector3D b)
        {
            return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
        }

        public static Vector3D Cross(Vector3D u, Vector3D v)
        {
            return new Vector3D(
                (u.Y * v.Z) - (u.Z * v.Y),
                (u.Z * v.X) - (u.X * v.Z),
                (u.X * v.Y) - (u.Y * v.X));
        }

        public double LengthSquared()
        {
            return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
        }

        public double Length()
        {
            return Math.Sqrt(this.LengthSquared());
        }

        public void Normalize()
        {
            var length = this.Length();

            this.X /= length;
            this.Y /= length;
            this.Z /= length;
        }

        public Vector3D Normalized()
        {
            var length = this.Length();

            return new Vector3D(
                this.X / length,
                this.Y / length,
                this.Z / length);
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", this.X, this.Y, this.Z);
        }
    }
}
