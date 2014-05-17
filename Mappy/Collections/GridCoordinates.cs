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

        public int X { get; private set; }

        public int Y { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            GridCoordinates g = (GridCoordinates)obj;

            return g.X == this.X && g.Y == this.Y;
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