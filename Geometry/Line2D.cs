namespace Geometry
{
    public struct Line2D
    {
        public Line2D(Vector2D start, Vector2D end)
            : this()
        {
            this.Start = start;
            this.End = end;
        }

        public Vector2D Start { get; set; }

        public Vector2D End { get; set; }

        public Ray2D ToRay()
        {
            return new Ray2D(this.Start, this.End - this.Start);
        }

        public Vector2D PointAt(double d)
        {
            return this.Start + ((this.End - this.Start) * d);
        }
    }
}
