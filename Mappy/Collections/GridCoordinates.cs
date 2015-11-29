namespace Mappy.Collections
{
    /// <summary>
    /// Represents the coordinates of a grid cell.
    /// </summary>
    public struct GridCoordinates
    {
        public GridCoordinates(int x, int y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public static bool operator ==(GridCoordinates a, GridCoordinates b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(GridCoordinates a, GridCoordinates b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            GridCoordinates g = (GridCoordinates)obj;

            return this == g;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", this.X, this.Y);
        }
    }
}