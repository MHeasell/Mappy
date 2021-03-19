namespace Mappy.Data
{
    using System;
    using System.Drawing;

    using Mappy.Collections;
    using Mappy.Util;

    /// <summary>
    /// Represents the "blueprint" for a feature.
    /// Contains metadata about the feature.
    /// </summary>
    public class Feature
    {
        public struct ReclaimInfoStruct
        {
            public int MetalValue { get; set; }

            public int EnergyValue { get; set; }
        }

        public string Name { get; set; }

        public string World { get; set; }

        public string Category { get; set; }

        public Size Footprint { get; set; }

        public Point Offset { get; set; }

        public Bitmap Image { get; set; }

        public Maybe<ReclaimInfoStruct> ReclaimInfo { get; set; }

        public Rectangle GetDrawBounds(IGrid<int> heightmap, int posX, int posY)
        {
            var height = 0;
            if (posX >= 0 && posX < heightmap.Width - 1 && posY >= 0 && posY < heightmap.Height - 1)
            {
                height = Util.ComputeMidpointHeight(heightmap, posX, posY);
            }

            var projectedPosX = (posX * 16) + (this.Footprint.Width * 8);
            var projectedPosY = (posY * 16) + (this.Footprint.Height * 8) - (height / 2);

            var pos = new Point(projectedPosX - this.Offset.X, projectedPosY - this.Offset.Y);
            return new Rectangle(pos, this.Image.Size);
        }
    }
}
