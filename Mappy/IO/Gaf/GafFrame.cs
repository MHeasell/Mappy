namespace Mappy.IO.Gaf
{
    using System;

    /// <summary>
    /// Data structure representing an individual frame
    /// in a GAF animation.
    /// </summary>
    public class GafFrame
    {
        /// <summary>
        /// Gets or sets the frame's offset in the X direction.
        /// </summary>
        public int OffsetX { get; set; }

        /// <summary>
        /// Gets or sets the frame's offset in the Y direction.
        /// </summary>
        public int OffsetY { get; set; }

        /// <summary>
        /// Gets or sets the width of the frame.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the frame.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets The color index used in this frame
        /// to represent transparency.
        /// </summary>
        public byte TransparencyIndex { get; set; }

        /// <summary>
        /// Gets or sets the frame's image data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
