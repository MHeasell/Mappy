namespace Geometry
{
    public struct Ray2D
    {
        public Ray2D(Vector2D origin, Vector2D direction) : this()
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        public Vector2D Origin { get; set; }

        public Vector2D Direction { get; set; }

        public Vector2D PointAt(double t)
        {
            return this.Origin + (this.Direction * t);
        }
    }
}
