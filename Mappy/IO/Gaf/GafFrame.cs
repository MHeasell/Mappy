namespace Mappy.IO.Gaf
{
    /// <summary>
    /// Data structure representing an individual frame
    /// in a GAF animation.
    /// </summary>
    public struct GafFrame
    {
        /// <summary>
        /// The frame's offset in the X direction.
        /// </summary>
        public int OffsetX;

        /// <summary>
        /// The frame's offset in the Y direction.
        /// </summary>
        public int OffsetY;

        /// <summary>
        /// The width of the frame.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the frame.
        /// </summary>
        public int Height;

        /// <summary>
        /// The color index used in this frame
        /// to represent transparency.
        /// </summary>
        public byte TransparencyIndex;

        /// <summary>
        /// The frame's image data.
        /// </summary>
        public byte[] Data;
    }
}
