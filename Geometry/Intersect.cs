namespace Geometry
{
    using System;

    public static class Intersect
    {
        public static bool TrianglePlane(Triangle3D t, Plane3D plane, out Line3D intersect)
        {
            var abLine = new Line3D(t.A, t.B);
            var acLine = new Line3D(t.A, t.C);
            var bcLine = new Line3D(t.B, t.C);

            double abIntersect;
            double acIntersect;
            double bcIntersect;

            if (plane.IntersectLine(abLine, out abIntersect))
            {
                if (plane.IntersectLine(acLine, out acIntersect))
                {
                    intersect = new Line3D(
                        abLine.PointAt(abIntersect),
                        acLine.PointAt(acIntersect));
                    return true;
                }
                else if (plane.IntersectLine(bcLine, out bcIntersect))
                {
                    intersect = new Line3D(
                        abLine.PointAt(abIntersect),
                        bcLine.PointAt(bcIntersect));
                    return true;
                }
                else
                {
                    intersect = new Line3D();
                    return false;
                }
            }
            else if (plane.IntersectLine(bcLine, out bcIntersect))
            {
                if (plane.IntersectLine(acLine, out acIntersect))
                {
                    intersect = new Line3D(
                        bcLine.PointAt(bcIntersect),
                        acLine.PointAt(acIntersect));
                    return true;
                }
                else
                {
                    throw new ArgumentException("triangle appears invalid");
                }
            }
            else
            {
                intersect = new Line3D();
                return false;
            }
        }

        public static double CompareToPlane(AxisRectangle3D rect, Plane3D plane)
        {
            var behind = 0;
            var infront = 0;

            foreach (var v in new[] { rect.TopLeft, rect.TopRight, rect.BottomLeft, rect.BottomRight })
            {
                var cmp = plane.TestSide(v);
                if (cmp < 0)
                {
                    behind++;
                }
                else
                {
                    infront++;
                }
            }

            if (behind > 0 && infront > 0)
            {
                return 0;
            }

            if (behind > 0)
            {
                return -1;
            }

            if (infront > 0)
            {
                return 1;
            }

            throw new ArgumentException("rect appears to be invalid");
        }
    }
}
