namespace Geometry
{
    using System;

    public struct Vector2D
    {
        public static readonly Vector2D XAxis = new Vector2D(1.0, 0.0);
        public static readonly Vector2D YAxis = new Vector2D(0.0, 1.0);

        public static readonly Vector2D Zero = new Vector2D(0.0, 0.0);

        public Vector2D(double x, double y) : this()
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2D operator -(Vector2D v)
        {
            return new Vector2D(-v.X, -v.Y);
        }

        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2D operator *(Vector2D v, double scalar)
        {
            return new Vector2D(v.X * scalar, v.Y * scalar);
        }

        public static Vector2D operator /(Vector2D v, double scalar)
        {
            return new Vector2D(v.X / scalar, v.Y / scalar);
        }

        public static double Dot(Vector2D a, Vector2D b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        public double LengthSquared()
        {
            return (this.X * this.X) + (this.Y * this.Y);
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
        }

        public Vector2D Normalized()
        {
            var length = this.Length();

            return new Vector2D(
                this.X / length,
                this.Y / length);
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", this.X, this.Y);
        }
    }
}
