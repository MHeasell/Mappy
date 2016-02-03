namespace Mappy.Operations
{
    using System;
    using System.Drawing;

    using Mappy.Data;

    public class MoveTileOperation : IReplayableOperation
    {
        public MoveTileOperation(Positioned<IMapTile> tile, int x, int y)
        {
            this.Tile = tile;
            this.X = x;
            this.Y = y;
        }

        public Positioned<IMapTile> Tile { get; }

        public int X { get; }

        public int Y { get; set; }

        public void Execute()
        {
            Point p = this.Tile.Location;
            p.X += this.X;
            p.Y += this.Y;
            this.Tile.Location = p;
        }

        public void Undo()
        {
            Point p = this.Tile.Location;
            p.X -= this.X;
            p.Y -= this.Y;
            this.Tile.Location = p;
        }

        public MoveTileOperation Combine(MoveTileOperation other)
        {
            if (other.Tile != this.Tile)
            {
                throw new ArgumentException("Operation not for the same tile");
            }

            return new MoveTileOperation(this.Tile, this.X + other.X, this.Y + other.Y);
        }
    }
}
