namespace Geometry
{
    public struct Rectangle2D
    {
        public static readonly Rectangle2D Empty = new Rectangle2D();

        public Rectangle2D(double x, double y, double width, double height)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
