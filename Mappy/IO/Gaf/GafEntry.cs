namespace Mappy.IO.Gaf
{
    /// <summary>
    /// Data structure representing an individual animation entry
    /// inside a GAF file.
    /// </summary>
    public class GafEntry
    {
        /// <summary>
        /// Gets or sets the name of the entry.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the array of animation frames in the entry.
        /// </summary>
        public GafFrame[] Frames { get; set; }
    }
}
