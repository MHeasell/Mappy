namespace Mappy.Collections
{
    public static class GridMethods
    {
        public static int ToIndex<T>(this IGrid<T> grid, int x, int y)
        {
            return (y * grid.Width) + x;
        }

        public static GridCoordinates ToCoords<T>(this IGrid<T> grid, int index)
        {
            return new GridCoordinates(index % grid.Width, index / grid.Width);
        }

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
}
