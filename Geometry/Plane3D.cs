namespace Geometry
{
    public struct Plane3D
    {
        public Plane3D(Vector3D point, Vector3D normal) : this()
        {
            this.Point = point;
            this.Normal = normal;
        }

        public Vector3D Point { get; set; }

        public Vector3D Normal { get; set; }

        public double Intersect(Ray3D ray)
        {
            double a = Vector3D.Dot(this.Point - ray.Origin, this.Normal);
            double b = Vector3D.Dot(ray.Direction, this.Normal);
            return a / b;
        }

        public bool IntersectLine(Line3D line, out double intersectDistance)
        {
            intersectDistance = this.Intersect(line.ToRay());
            return intersectDistance >= 0.0 && intersectDistance <= 1.0;
        }

        /// <summary>
        /// Determines where a point is relative to this plane.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>less than 0 if the point is behind the plane, 0 if it is on the plane, greater than 0 if it is in front of the plane</returns>
        public double TestSide(Vector3D point)
        {
            return Vector3D.Dot(point - this.Point, this.Normal);
        }
    }
}
