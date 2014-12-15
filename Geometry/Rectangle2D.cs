namespace Geometry
{
    public struct Rectangle2D
    {
        public static readonly Rectangle2D Empty = new Rectangle2D();

        public Rectangle2D(Vector2D center, Vector2D extents)
            : this()
        {
            this.Center = center;
            this.Extents = extents;
        }

        public Vector2D Center { get; set; }

        public Vector2D Extents { get; set; }

        public double ExtentX
        {
            get
            {
                return this.Extents.X;
            }

            set
            {
                this.Extents = new Vector2D(value, this.ExtentY);
            }
        }

        public double ExtentY
        {
            get
            {
                return this.Extents.Y;
            }

            set
            {
                this.Extents = new Vector2D(this.ExtentX, value);
            }
        }

        public Vector2D Size
        {
            get
            {
                return this.Extents * 2.0;
            }
        }

        public double Width
        {
            get
            {
                return this.Size.X;
            }
        }

        public double Height
        {
            get
            {
                return this.Size.Y;
            }
        }

        public double CenterX
        {
            get
            {
                return this.Center.X;
            }

            set
            {
                this.Center = new Vector2D(value, this.CenterY);
            }
        }

        public double CenterY
        {
            get
            {
                return this.Center.Y;
            }

            set
            {
                this.Center = new Vector2D(this.CenterX, value);
            }
        }

        public double MinX
        {
            get
            {
                return this.Center.X - this.Extents.X;
            }
        }

        public double MinY
        {
            get
            {
                return this.Center.Y - this.Extents.Y;
            }
        }

        public double MaxX
        {
            get
            {
                return this.Center.Y + this.Extents.Y;
            }
        }

        public double MaxY
        {
            get
            {
                return this.Center.X + this.Extents.X;
            }
        }

        public Vector2D MinXY
        {
            get
            {
                return new Vector2D(this.MinX, this.MinY);
            }
        }

        public Vector2D MaxXY
        {
            get
            {
                return new Vector2D(this.MaxX, this.MaxY);
            }
        }

        public static Rectangle2D FromCorner(Vector2D position, Vector2D size)
        {
            var halfSize = size / 2.0;
            return new Rectangle2D(position + halfSize, halfSize);
        }

        public static Rectangle2D FromCorner(
            double x,
            double y,
            double width,
            double height)
        {
            return FromCorner(new Vector2D(x, y), new Vector2D(width, height));
        }
    }
}
