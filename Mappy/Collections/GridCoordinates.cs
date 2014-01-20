namespace Mappy.Collections
{
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
    }
}